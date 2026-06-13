'use client';

import { ResourcePage, statusBadge, type ResourceCrud } from '../components/ResourcePage';
import { clinicalRecordsApi } from '../services/clinicalRecords';
import { useApp } from '../state/AppContext';
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

const recordsCrud: ResourceCrud<MedicalRecord> = {
  create: (record) => clinicalRecordsApi.create(toPayload(record)),
  update: (id, record) => clinicalRecordsApi.update(Number(id), toPayload(record)),
  remove: () => Promise.resolve(),
};

function toPayload(record: MedicalRecord): MedicalRecord {
  return {
    ...record,
    name: record.name || record.patientName,
    status: record.status,
  };
}

export function RecordsPage() {
  const { data, setData } = useApp();
  const onRecordsChange = (records: MedicalRecord[]) => setData((current) => ({ ...current, records }));

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
      crud={recordsCrud}
      fields={fields}
      lookups={buildLookups(data)}
      emptyValue={emptyRecord}
      getId={(record) => record.id}
      getTitle={(record) => record.name || record.patientName || `Prontuario ${record.id}`}
      onItemsChange={onRecordsChange}
      toCreatePayload={toPayload}
      toUpdatePayload={toPayload}
      columns={[
        {
          key: 'patientName',
          header: 'Paciente e documento',
          render: (record) => (
            <div className="person-cell">
              <strong>{record.patientName || `Paciente #${record.patientId}`}</strong>
              <span>{record.name || 'Documento clinico sem nome'}</span>
            </div>
          ),
        },
        { key: 'updatedAt', header: 'Ultima atualizacao', render: (record) => formatUpdatedAt(record.updatedAt) },
        { key: 'nextStep', header: 'Proximo passo', render: (record) => nextStepLabel(record) },
        { key: 'status', header: 'Status', render: (record) => statusBadge(record.status) },
      ]}
      detailFields={[
        { label: 'Paciente', value: (record) => record.patientName || `Paciente #${record.patientId}` },
        { label: 'Documento', value: (record) => record.name || 'Documento clinico sem nome' },
        { label: 'Ultima atualizacao', value: (record) => formatUpdatedAt(record.updatedAt) },
        { label: 'Sessao para evolucao', value: (record) => (findSessionId(data, record.patientId) ? `Sessao #${findSessionId(data, record.patientId)}` : 'Nenhuma sessao vinculada') },
        { label: 'Status', value: (record) => statusBadge(record.status) },
      ]}
      actions={[
        { label: 'Ler anamnese', successMessage: 'Anamnese carregada.', run: (record) => clinicalRecordsApi.anamnesis(record.id) },
        { label: 'Salvar rascunho de anamnese', successMessage: 'Rascunho de anamnese salvo.', run: (record) => clinicalRecordsApi.autosaveAnamnesis(record.id, { ciphertext: 'draft', nonce: 'dev', tag: 'dev' }) },
        { label: 'Publicar anamnese', successMessage: 'Versao de anamnese publicada.', run: (record) => clinicalRecordsApi.publishAnamnesis(record.id, { reason: 'Publicada pelo workspace web' }) },
        { label: 'Salvar rascunho de evolucao', successMessage: 'Rascunho de evolucao salvo.', run: (record) => clinicalRecordsApi.autosaveEvolution(requireSessionId(record), { ciphertext: 'draft', nonce: 'dev', tag: 'dev' }) },
        { label: 'Publicar evolucao', successMessage: 'Versao de evolucao publicada.', run: (record) => clinicalRecordsApi.publishEvolution(requireSessionId(record), { reason: 'Publicada pelo workspace web' }) },
        { label: 'Historico de evolucao', successMessage: 'Historico de evolucao carregado.', run: (record) => clinicalRecordsApi.evolutionVersions(requireSessionId(record)) },
        { label: 'Ver auditoria', successMessage: 'Auditoria carregada.', run: (record) => clinicalRecordsApi.auditLog(record.id) },
      ]}
      summaryLabel={(count) => `${count} ${count === 1 ? 'prontuario' : 'prontuarios'}`}
      summaryDescription={(count) =>
        count === 0 ? 'Nenhum documento clinico cadastrado.' : 'Revise documentos, publique versoes e consulte auditoria com contexto do paciente.'
      }
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
  return new Date(value).toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

function nextStepLabel(record: MedicalRecord) {
  const status = record.status.toLowerCase();
  if (status.includes('assinado')) return 'Consultar ou auditar';
  if (status.includes('revis')) return 'Revisar antes de publicar';
  return 'Continuar documentacao';
}
