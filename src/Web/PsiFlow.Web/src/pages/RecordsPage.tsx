import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { api } from '../services/api';
import type { DashboardData, FormField, LookupMap, MedicalRecord } from '../types';

const fields: Array<FormField<MedicalRecord>> = [
  { name: 'patientId', label: 'Paciente', type: 'lookup', lookupKey: 'patients', required: true, helperText: 'Paciente vinculado a este prontuario.' },
  { name: 'name', label: 'Documento clinico', type: 'text', required: true, placeholder: 'Ex: Anamnese inicial' },
  { name: 'status', label: 'Status', type: 'select', required: true, options: [{ label: 'Rascunho', value: 'Rascunho' }, { label: 'Assinado', value: 'Assinado' }, { label: 'Revisar', value: 'Revisar' }] },
];

const emptyRecord: MedicalRecord = { id: 0, tenantId: 1, patientId: 0, name: '', patientName: '', updatedAt: 'Agora', status: 'Rascunho' };

function buildLookups(data: DashboardData): LookupMap {
  return {
    patients: data.patients.map((patient) => ({ label: patient.fullName ?? patient.name, value: patient.id })),
  };
}

type RecordsPageProps = { data: DashboardData; onRecordsChange: (records: MedicalRecord[]) => void };

export function RecordsPage({ data, onRecordsChange }: RecordsPageProps) {
  function requireSessionId(record: MedicalRecord) {
    const sessionId = findSessionId(data, record.patientId);
    if (!sessionId) {
      throw new Error('Nenhuma sessao vinculada a este paciente. Crie ou selecione uma sessao antes de registrar evolucao.');
    }
    return sessionId;
  }

  return (
    <ResourcePage
      title="Prontuarios"
      description="Documentos clinicos, revisoes, anamnese, evolucao e auditoria por paciente."
      createLabel="Novo prontuario"
      items={data.records}
      service="clinicalRecords"
      path="/v1/clinical-records"
      fields={fields}
      lookups={buildLookups(data)}
      emptyValue={emptyRecord}
      getId={(record) => record.id}
      getTitle={(record) => record.name || record.patientName || `Prontuario ${record.id}`}
      onItemsChange={onRecordsChange}
      toCreatePayload={(record) => ({ tenantId: record.tenantId, patientId: record.patientId, name: record.name || record.patientName, status: record.status === 'Assinado' ? 'signed' : 'active' })}
      toUpdatePayload={(record) => ({ id: record.id, tenantId: record.tenantId, patientId: record.patientId, name: record.name || record.patientName, status: record.status === 'Assinado' ? 'signed' : 'active' })}
      columns={[
        { key: 'patientName', header: 'Paciente e documento', render: (record) => <div className="person-cell"><strong>{record.patientName || `Paciente #${record.patientId}`}</strong><span>{record.name || 'Documento clinico sem nome'}</span></div> },
        { key: 'updatedAt', header: 'Ultima atualizacao', render: (record) => formatUpdatedAt(record.updatedAt) },
        { key: 'nextStep', header: 'Proximo passo', render: (record) => nextStepLabel(record) },
        { key: 'status', header: 'Status', render: (record) => statusBadge(record.status) },
      ]}
      detailFields={[
        { label: 'Paciente', value: (record) => record.patientName || `Paciente #${record.patientId}` },
        { label: 'Documento', value: (record) => record.name || 'Documento clinico sem nome' },
        { label: 'Ultima atualizacao', value: (record) => formatUpdatedAt(record.updatedAt) },
        { label: 'Sessao para evolucao', value: (record) => findSessionId(data, record.patientId) ? `Sessao #${findSessionId(data, record.patientId)}` : 'Nenhuma sessao vinculada' },
        { label: 'Status', value: (record) => statusBadge(record.status) },
      ]}
      actions={[
        { label: 'Ler anamnese', successMessage: 'Anamnese carregada.', run: (record) => api.get('clinicalRecords', `/v1/clinical-records/${record.id}/anamnesis`) },
        { label: 'Salvar rascunho de anamnese', successMessage: 'Rascunho de anamnese salvo.', run: (record) => api.post('clinicalRecords', `/v1/clinical-records/${record.id}/anamnesis/autosave`, { ciphertext: 'draft', nonce: 'dev', tag: 'dev' }) },
        { label: 'Publicar anamnese', successMessage: 'Versao de anamnese publicada.', run: (record) => api.post('clinicalRecords', `/v1/clinical-records/${record.id}/anamnesis/publish-version`, { reason: 'Publicada pelo workspace web' }) },
        { label: 'Salvar rascunho de evolucao', successMessage: 'Rascunho de evolucao salvo.', run: (record) => api.post('clinicalRecords', `/v1/sessions/${requireSessionId(record)}/evolution/autosave`, { ciphertext: 'draft', nonce: 'dev', tag: 'dev' }) },
        { label: 'Publicar evolucao', successMessage: 'Versao de evolucao publicada.', run: (record) => api.post('clinicalRecords', `/v1/sessions/${requireSessionId(record)}/evolution/publish-version`, { reason: 'Publicada pelo workspace web' }) },
        { label: 'Historico de evolucao', successMessage: 'Historico de evolucao carregado.', run: (record) => api.get('clinicalRecords', `/v1/sessions/${requireSessionId(record)}/evolution/versions`) },
        { label: 'Ver auditoria', successMessage: 'Auditoria carregada.', run: (record) => api.get('clinicalRecords', `/v1/clinical-records/${record.id}/audit-log`) },
      ]}
      summaryLabel={(count) => `${count} ${count === 1 ? 'prontuario' : 'prontuarios'}`}
      summaryDescription={(count) => count === 0 ? 'Nenhum documento clinico cadastrado.' : 'Revise documentos, publique versoes e consulte auditoria com contexto do paciente.'}
      updatePolicyLabel="Registra versoes e auditoria"
      emptyTitle="Nenhum prontuario cadastrado"
      emptyDescription="Crie o primeiro prontuario de um paciente para registrar anamnese, evolucao e historico clinico com rastreabilidade."
      modalDescription="Selecione o paciente e nomeie o documento clinico. Conteudo sensivel deve ser salvo nas acoes de anamnese ou evolucao."
      createSubmitLabel="Criar prontuario"
      editSubmitLabel="Salvar prontuario"
      detailEditLabel="Editar dados do prontuario"
      moreActionsLabel="Anamnese, evolucao e auditoria"
      moreActionsHint="Publicar cria versao clinica registrada. Evolucao exige uma sessao vinculada ao paciente."
    />
  );
}

function findSessionId(data: DashboardData, patientId?: number) {
  return data.sessions.find((session) => session.patientId === patientId)?.id;
}

function formatUpdatedAt(value?: string) {
  if (!value) return 'Nao informado';
  if (value === 'Agora') return value;
  return new Date(value).toLocaleString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
}

function nextStepLabel(record: MedicalRecord) {
  const status = record.status.toLowerCase();
  if (status.includes('assinado')) return 'Consultar ou auditar';
  if (status.includes('revis')) return 'Revisar antes de publicar';
  return 'Continuar documentacao';
}
