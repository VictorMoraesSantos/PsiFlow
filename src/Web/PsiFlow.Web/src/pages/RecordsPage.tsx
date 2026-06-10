import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { api } from '../services/api';
import type { DashboardData, FormField, LookupMap, MedicalRecord } from '../types';

const fields: Array<FormField<MedicalRecord>> = [
  { name: 'patientId', label: 'Paciente', type: 'lookup', lookupKey: 'patients', required: true, helperText: 'Paciente dono deste prontuario.' },
  { name: 'name', label: 'Nome do prontuario', type: 'text', required: true, placeholder: 'Ex: Anamnese inicial' },
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
  return (
    <ResourcePage
      title="Prontuarios"
      description="Registros clinicos, revisoes, anamnese e evolucao."
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
        { key: 'patientName', header: 'Prontuario', render: (record) => <strong>{record.name || record.patientName}</strong> },
        { key: 'updatedAt', header: 'Atualizacao', render: (record) => record.updatedAt },
        { key: 'status', header: 'Status', render: (record) => statusBadge(record.status) },
      ]}
      detailFields={[
        { label: 'Paciente', value: (record) => record.patientName || `Paciente #${record.patientId}` },
        { label: 'Nome', value: (record) => record.name || record.patientName },
        { label: 'Atualizacao', value: (record) => record.updatedAt },
        { label: 'Status', value: (record) => statusBadge(record.status) },
      ]}
      actions={[
        { label: 'Autosave anamnese', successMessage: 'Rascunho salvo ou acao simulada.', run: (record) => api.post('clinicalRecords', `/v1/clinical-records/${record.id}/anamnesis/autosave`, { ciphertext: 'draft', nonce: 'dev', tag: 'dev' }) },
        { label: 'Publicar versao', successMessage: 'Versao publicada ou simulada.', run: (record) => api.post('clinicalRecords', `/v1/clinical-records/${record.id}/anamnesis/publish-version`, { reason: 'Publicada pelo workspace web' }) },
        { label: 'Ver auditoria', successMessage: 'Auditoria carregada ou simulada.', run: (record) => api.get('clinicalRecords', `/v1/clinical-records/${record.id}/audit-log`) },
      ]}
    />
  );
}
