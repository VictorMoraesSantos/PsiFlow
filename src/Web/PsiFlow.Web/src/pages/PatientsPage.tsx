import { CalendarPlus, Edit3, Eye, FileText, Search, ShieldAlert, Trash2, UserPlus } from 'lucide-react';
import { useMemo, useState } from 'react';
import { Badge } from '../components/Badge';
import { Button } from '../components/Button';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { DetailDrawer } from '../components/DetailDrawer';
import { ResourceFormModal } from '../components/ResourceFormModal';
import { Section } from '../components/Section';
import { useToast } from '../components/Toast';
import { api, createResource, updateResource } from '../services/api';
import type { DashboardData, FormField, Patient, StatusTone } from '../types';

const fields: Array<FormField<Patient>> = [
  { name: 'fullName', label: 'Nome completo', type: 'text', required: true, placeholder: 'Como aparece no cadastro' },
  { name: 'email', label: 'Email', type: 'email', required: true, placeholder: 'paciente@email.com' },
  { name: 'phone', label: 'Telefone', type: 'tel', required: true, placeholder: '(11) 90000-0000' },
  { name: 'birthDate', label: 'Nascimento', type: 'date' },
  { name: 'status', label: 'Status', type: 'select', required: true, options: [{ label: 'Ativo', value: 'Ativo' }, { label: 'Aguardando', value: 'Aguardando' }, { label: 'Inativo', value: 'Inativo' }] },
  { name: 'treatmentStatus', label: 'Tratamento', type: 'select', options: [{ label: 'Triagem', value: 'screening' }, { label: 'Ativo', value: 'active' }, { label: 'Alta', value: 'discharged' }] },
  { name: 'emergencyContactName', label: 'Contato de emergencia', type: 'text', placeholder: 'Nome do contato' },
  { name: 'emergencyContactPhone', label: 'Telefone de emergencia', type: 'tel', placeholder: '(11) 90000-0000' },
];

const emptyPatient: Patient = { id: 0, tenantId: 1, name: '', fullName: '', email: '', phone: '', birthDate: null, status: 'Ativo', treatmentStatus: 'screening', emergencyContactName: '', emergencyContactPhone: '', nextSession: 'Sem sessao agendada', risk: 'Baixo' };

type PatientsPageProps = {
  data: DashboardData;
  onPatientsChange: (patients: Patient[]) => void;
};

type PatientStatusFilter = 'Todos' | Patient['status'];

export function PatientsPage({ data, onPatientsChange }: PatientsPageProps) {
  const { notify } = useToast();
  const [query, setQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState<PatientStatusFilter>('Todos');
  const [selected, setSelected] = useState<Patient | null>(null);
  const [mode, setMode] = useState<'create' | 'edit' | null>(null);
  const [inactiveTarget, setInactiveTarget] = useState<Patient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const filteredPatients = useMemo(() => {
    const normalizedQuery = query.trim().toLowerCase();
    return data.patients.filter((patient) => {
      const matchesStatus = statusFilter === 'Todos' || patient.status === statusFilter;
      const searchable = [patient.fullName, patient.name, patient.email, patient.phone, patient.status, patient.treatmentStatus].filter(Boolean).join(' ').toLowerCase();
      return matchesStatus && (normalizedQuery.length === 0 || searchable.includes(normalizedQuery));
    });
  }, [data.patients, query, statusFilter]);

  const activeCount = data.patients.filter((patient) => patient.status === 'Ativo').length;
  const waitingCount = data.patients.filter((patient) => patient.status === 'Aguardando').length;
  const incompleteCount = data.patients.filter((patient) => !patient.email || !patient.phone || !patient.fullName).length;
  const formValue = mode === 'edit' && selected ? selected : emptyPatient;
  const hasNoPatients = data.patients.length === 0;
  const hasNoResults = data.patients.length > 0 && filteredPatients.length === 0;

  async function submit(value: Patient) {
    setIsSubmitting(true);
    try {
      if (mode === 'create') {
        const fallback = { ...value, id: Date.now(), name: value.fullName || value.name };
        const created = await createResource<Patient>('patients', '/v1/patients', toPayload(value), fallback);
        onPatientsChange([created, ...data.patients]);
        notify('Paciente cadastrado.');
      }
      if (mode === 'edit') {
        const updated = await updateResource<Patient>('patients', '/v1/patients', value.id, toPayload(value), { ...value, name: value.fullName || value.name });
        onPatientsChange(data.patients.map((patient) => patient.id === value.id ? updated : patient));
        notify('Cadastro do paciente atualizado.');
      }
      setMode(null);
      setSelected(null);
    } catch (error) {
      notify(error instanceof Error ? error.message : 'Nao foi possivel salvar o paciente.', 'danger');
    } finally {
      setIsSubmitting(false);
    }
  }

  async function confirmInactivate() {
    if (!inactiveTarget) return;
    try {
      await api.post('patients', `/v1/patients/${inactiveTarget.id}/deactivate`, { reason: 'Inativado pelo workspace web' });
      onPatientsChange(data.patients.map((patient) => patient.id === inactiveTarget.id ? { ...patient, status: 'Inativo' } : patient));
      notify('Paciente inativado.');
      setInactiveTarget(null);
      setSelected(null);
    } catch (error) {
      notify(error instanceof Error ? error.message : 'Nao foi possivel inativar o paciente.', 'danger');
    }
  }

  return (
    <div className="resource-layout patients-page">
      <Section
        title="Pacientes"
        description="Busca, cadastro e contexto operacional sem expor notas clinicas na lista."
        action={<Button type="button" onClick={() => { setSelected(null); setMode('create'); }}><UserPlus size={17} aria-hidden="true" />Novo paciente</Button>}
      >
        <div className="patients-summary" aria-label="Resumo de pacientes">
          <div><strong>{data.patients.length}</strong><span>pacientes cadastrados</span></div>
          <div><strong>{activeCount}</strong><span>ativos</span></div>
          <div><strong>{waitingCount}</strong><span>aguardando</span></div>
          <div><strong>{incompleteCount}</strong><span>cadastros incompletos</span></div>
        </div>

        <div className="patients-toolbar" aria-label="Busca e filtros de pacientes">
          <label className="input-with-icon patients-search">
            <Search aria-hidden="true" size={18} />
            <span className="sr-only">Buscar pacientes</span>
            <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Buscar por nome, telefone ou email" />
          </label>
          <div className="patients-filters" aria-label="Filtrar por status">
            {(['Todos', 'Ativo', 'Aguardando', 'Inativo'] as const).map((status) => (
              <button key={status} type="button" className={statusFilter === status ? 'filter-chip filter-chip--active' : 'filter-chip'} onClick={() => setStatusFilter(status)}>
                {status}
              </button>
            ))}
          </div>
        </div>

        <p className="privacy-note"><ShieldAlert aria-hidden="true" size={16} /> Dados clinicos detalhados ficam no prontuario. Esta lista mostra apenas informacoes operacionais.</p>

        {hasNoPatients ? <PatientsEmptyState onCreate={() => setMode('create')} /> : null}
        {hasNoResults ? <PatientsNoResults query={query} onClear={() => { setQuery(''); setStatusFilter('Todos'); }} /> : null}
        {!hasNoPatients && !hasNoResults ? (
          <PatientList
            patients={filteredPatients}
            onOpen={(patient) => setSelected(patient)}
            onEdit={(patient) => { setSelected(patient); setMode('edit'); }}
            onInactivate={setInactiveTarget}
          />
        ) : null}
      </Section>

      <DetailDrawer
        item={selected && mode === null ? selected : null}
        title={selected ? getPatientName(selected) : 'Paciente'}
        fields={[
          { label: 'Email', value: (patient) => patient.email || 'Nao informado' },
          { label: 'Telefone', value: (patient) => patient.phone || 'Nao informado' },
          { label: 'Nascimento', value: (patient) => patient.birthDate || 'Nao informado' },
          { label: 'Tratamento', value: (patient) => formatTreatment(patient.treatmentStatus) },
          { label: 'Proxima sessao', value: (patient) => patient.nextSession || 'Sem sessao agendada' },
          { label: 'Contato de emergencia', value: (patient) => patient.emergencyContactName || 'Nao informado' },
          { label: 'Status', value: (patient) => <Badge tone={getStatusTone(patient.status)}>{patient.status}</Badge> },
        ]}
        onClose={() => setSelected(null)}
        actions={selected ? (
          <>
            <Button type="button" variant="secondary"><FileText size={16} aria-hidden="true" />Ver prontuario</Button>
            <Button type="button" variant="secondary"><CalendarPlus size={16} aria-hidden="true" />Agendar sessao</Button>
            <Button type="button" variant="secondary" onClick={() => setMode('edit')}><Edit3 size={16} aria-hidden="true" />Editar cadastro</Button>
            <Button type="button" className="button--danger" onClick={() => setInactiveTarget(selected)}>Inativar paciente</Button>
          </>
        ) : null}
      />

      <ResourceFormModal
        isOpen={mode !== null}
        title={mode === 'create' ? 'Novo paciente' : `Editar ${selected ? getPatientName(selected) : 'paciente'}`}
        description="Preencha dados cadastrais e contatos. Notas clinicas devem permanecer no prontuario."
        fields={fields}
        initialValue={formValue}
        submitLabel={mode === 'create' ? 'Cadastrar paciente' : 'Salvar cadastro'}
        isSubmitting={isSubmitting}
        onClose={() => setMode(null)}
        onSubmit={submit}
      />

      <ConfirmDialog
        isOpen={inactiveTarget !== null}
        title="Inativar paciente"
        description={inactiveTarget ? `${getPatientName(inactiveTarget)} deixara de aparecer como paciente ativo. O historico clinico nao sera removido.` : ''}
        confirmLabel="Inativar paciente"
        isDanger
        onClose={() => setInactiveTarget(null)}
        onConfirm={confirmInactivate}
      />
    </div>
  );
}

function PatientList({ patients, onOpen, onEdit, onInactivate }: { patients: Patient[]; onOpen: (patient: Patient) => void; onEdit: (patient: Patient) => void; onInactivate: (patient: Patient) => void }) {
  return (
    <div className="patients-table-wrap">
      <table className="patients-table">
        <caption>Pacientes: lista operacional</caption>
        <thead>
          <tr>
            <th scope="col">Paciente</th>
            <th scope="col">Contato</th>
            <th scope="col">Tratamento</th>
            <th scope="col">Proxima sessao</th>
            <th scope="col">Status</th>
            <th scope="col">Acoes</th>
          </tr>
        </thead>
        <tbody>
          {patients.map((patient) => (
            <tr key={patient.id}>
              <td data-label="Paciente"><div className="person-cell"><strong>{getPatientName(patient)}</strong><span>{patient.email || 'Email nao informado'}</span></div></td>
              <td data-label="Contato">{patient.phone || 'Nao informado'}</td>
              <td data-label="Tratamento"><Badge tone="info">{formatTreatment(patient.treatmentStatus)}</Badge></td>
              <td data-label="Proxima sessao">{patient.nextSession || 'Sem sessao agendada'}</td>
              <td data-label="Status"><Badge tone={getStatusTone(patient.status)}>{patient.status}</Badge></td>
              <td data-label="Acoes">
                <div className="row-actions">
                  <button type="button" className="table-action" onClick={() => onOpen(patient)} aria-label={`Abrir detalhes de ${getPatientName(patient)}`}><Eye size={15} aria-hidden="true" /><span>Abrir</span></button>
                  <button type="button" className="table-action" onClick={() => onEdit(patient)} aria-label={`Editar ${getPatientName(patient)}`}><Edit3 size={15} aria-hidden="true" /><span>Editar</span></button>
                  <button type="button" className="table-action table-action--danger" onClick={() => onInactivate(patient)} aria-label={`Inativar ${getPatientName(patient)}`}><Trash2 size={15} aria-hidden="true" /><span>Inativar</span></button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function PatientsEmptyState({ onCreate }: { onCreate: () => void }) {
  return (
    <div className="empty-state patients-state">
      <UserPlus aria-hidden="true" size={34} />
      <h3>Nenhum paciente cadastrado</h3>
      <p>Cadastre o primeiro paciente para iniciar agenda, sessoes e prontuario com contexto seguro.</p>
      <Button type="button" onClick={onCreate}>Cadastrar paciente</Button>
    </div>
  );
}

function PatientsNoResults({ query, onClear }: { query: string; onClear: () => void }) {
  return (
    <div className="empty-state patients-state">
      <Search aria-hidden="true" size={34} />
      <h3>Nenhum paciente encontrado</h3>
      <p>{query ? `A busca por "${query}" nao encontrou cadastro correspondente.` : 'Os filtros atuais nao retornaram pacientes.'}</p>
      <Button type="button" variant="secondary" onClick={onClear}>Limpar busca</Button>
    </div>
  );
}

function getPatientName(patient: Patient) {
  return patient.fullName || patient.name || 'Paciente sem nome';
}

function getStatusTone(status: Patient['status']): StatusTone {
  if (status === 'Ativo') return 'success';
  if (status === 'Inativo') return 'danger';
  return 'warning';
}

function formatTreatment(status?: string) {
  if (status === 'active') return 'Ativo';
  if (status === 'discharged') return 'Alta';
  return 'Triagem';
}

function toPayload(patient: Patient) {
  return {
    id: patient.id,
    tenantId: patient.tenantId,
    fullName: patient.fullName || patient.name,
    email: patient.email,
    phone: patient.phone,
    birthDate: patient.birthDate || null,
    emergencyContactName: patient.emergencyContactName || null,
    emergencyContactPhone: patient.emergencyContactPhone || null,
    status: patient.status === 'Inativo' ? 'inactive' : 'active',
    treatmentStatus: patient.treatmentStatus || 'screening',
  };
}
