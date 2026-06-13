'use client';

import { CalendarPlus, Edit3, Eye, FileText, Search, ShieldAlert, Trash2, UserPlus } from 'lucide-react';
import { useMemo, useState } from 'react';
import { Badge } from '../components/Badge';
import { Button } from '../components/Button';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { DetailDrawer } from '../components/DetailDrawer';
import { ResourceFormModal } from '../components/ResourceFormModal';
import { Section } from '../components/Section';
import { useToast } from '../components/Toast';
import { isLocalFallbackStatus } from '../services/http';
import { patientsApi } from '../services/patients';
import { useApp } from '../state/AppContext';
import type { FormField, Patient, StatusTone } from '../types';

const fields: Array<FormField<Patient>> = [
  { name: 'fullName', label: 'Nome completo', type: 'text', required: true, placeholder: 'Como aparece no cadastro' },
  { name: 'email', label: 'Email', type: 'email', required: true, placeholder: 'paciente@email.com' },
  { name: 'phone', label: 'Telefone', type: 'tel', required: true, placeholder: '(11) 90000-0000' },
  { name: 'birthDate', label: 'Nascimento', type: 'date' },
  { name: 'status', label: 'Status', type: 'select', required: true, options: [{ label: 'Ativo', value: 'Ativo' }, { label: 'Aguardando', value: 'Aguardando' }, { label: 'Inativo', value: 'Inativo' }] },
  { name: 'treatmentStatus', label: 'Tratamento', type: 'select', options: [{ label: 'Triagem', value: 'screening' }, { label: 'Em tratamento', value: 'active_treatment' }, { label: 'Pausa', value: 'paused' }, { label: 'Alta', value: 'discharged' }] },
  { name: 'emergencyContactName', label: 'Contato de emergencia', type: 'text', placeholder: 'Nome do contato' },
  { name: 'emergencyContactPhone', label: 'Telefone de emergencia', type: 'tel', placeholder: '(11) 90000-0000' },
];

const emptyPatient: Patient = {
  id: 0,
  tenantId: 1,
  name: '',
  fullName: '',
  email: '',
  phone: '',
  birthDate: null,
  status: 'Ativo',
  treatmentStatus: 'screening',
  emergencyContactName: '',
  emergencyContactPhone: '',
  nextSession: 'Sem sessao agendada',
  risk: 'Baixo',
};

type PatientStatusFilter = 'Todos' | Patient['status'];

export function PatientsPage() {
  const { data, setData } = useApp();
  const onPatientsChange = (patients: Patient[]) => setData((current) => ({ ...current, patients }));
  const { notify } = useToast();
  const [query, setQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState<PatientStatusFilter>('Todos');
  const [selected, setSelected] = useState<Patient | null>(null);
  const [mode, setMode] = useState<'create' | 'edit' | null>(null);
  const [inactiveTarget, setInactiveTarget] = useState<Patient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [pendingAction, setPendingAction] = useState<string | null>(null);

  const filteredPatients = useMemo(() => {
    const normalizedQuery = query.trim().toLowerCase();
    return data.patients.filter((patient) => {
      const matchesStatus = statusFilter === 'Todos' || patient.status === statusFilter;
      const searchable = [patient.fullName, patient.name, patient.email, patient.phone, patient.status, patient.treatmentStatus]
        .filter(Boolean)
        .join(' ')
        .toLowerCase();
      return matchesStatus && (normalizedQuery.length === 0 || searchable.includes(normalizedQuery));
    });
  }, [data.patients, query, statusFilter]);

  const activeCount = data.patients.filter((patient) => patient.status === 'Ativo').length;
  const waitingCount = data.patients.filter((patient) => patient.status === 'Aguardando').length;
  const incompleteCount = data.patients.filter(
    (patient) => !patient.email || !patient.phone || !patient.fullName,
  ).length;
  const formValue = mode === 'edit' && selected ? selected : emptyPatient;
  const hasNoPatients = data.patients.length === 0;
  const hasNoResults = data.patients.length > 0 && filteredPatients.length === 0;

  async function submit(value: Patient) {
    setIsSubmitting(true);
    try {
      if (mode === 'create') {
        const created = await patientsApi.create(toPayload(value));
        onPatientsChange([created, ...data.patients]);
        notify('Paciente cadastrado.');
      }
      if (mode === 'edit') {
        const updated = await patientsApi.update(value.id, toPayload(value));
        onPatientsChange(data.patients.map((patient) => (patient.id === value.id ? updated : patient)));
        notify('Cadastro do paciente atualizado.');
      }
      setMode(null);
      setSelected(null);
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        const optimistic = { ...toPayload(value), id: mode === 'create' ? Date.now() : value.id } as Patient;
        if (mode === 'create') onPatientsChange([optimistic, ...data.patients]);
        if (mode === 'edit') onPatientsChange(data.patients.map((patient) => (patient.id === value.id ? optimistic : patient)));
        notify('Paciente salvo em modo local.', 'info');
        setMode(null);
        setSelected(null);
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel salvar o paciente.', 'danger');
    } finally {
      setIsSubmitting(false);
    }
  }

  async function confirmInactivate() {
    if (!inactiveTarget) return;
    const actionKey = `inactivate:${inactiveTarget.id}`;
    setPendingAction(actionKey);
    try {
      await patientsApi.deactivate(inactiveTarget.id, 'Inativado pelo workspace web');
      onPatientsChange(
        data.patients.map((patient) =>
          patient.id === inactiveTarget.id ? { ...patient, status: 'Inativo' as const } : patient,
        ),
      );
      notify('Paciente inativado.');
      setInactiveTarget(null);
      setSelected(null);
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        onPatientsChange(
          data.patients.map((patient) =>
            patient.id === inactiveTarget.id ? { ...patient, status: 'Inativo' as const } : patient,
          ),
        );
        notify('Paciente inativado localmente.', 'info');
        setInactiveTarget(null);
        setSelected(null);
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel inativar o paciente.', 'danger');
    } finally {
      setPendingAction(null);
    }
  }

  async function invitePatient(patient: Patient) {
    if (!patient.email || !patient.phone) {
      notify('Informe email e telefone antes de enviar o convite.', 'danger');
      return;
    }
    const actionKey = `invite:${patient.id}`;
    setPendingAction(actionKey);
    try {
      await patientsApi.invite({ patientId: patient.id, email: patient.email, phone: patient.phone });
      onPatientsChange(
        data.patients.map((item) =>
          item.id === patient.id ? { ...item, status: (item.status === 'Ativo' ? item.status : 'Aguardando') as Patient['status'] } : item,
        ),
      );
      notify('Convite do paciente criado.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        onPatientsChange(
          data.patients.map((item) =>
            item.id === patient.id ? { ...item, status: 'Aguardando' as Patient['status'] } : item,
          ),
        );
        notify('Convite registrado localmente.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel criar o convite.', 'danger');
    } finally {
      setPendingAction(null);
    }
  }

  async function changeTreatmentStatus(patient: Patient, treatmentStatus: string) {
    if (patient.treatmentStatus === treatmentStatus) {
      notify(`${getPatientName(patient)} ja esta com este status de tratamento.`, 'info');
      return;
    }
    const actionKey = `treatment:${patient.id}:${treatmentStatus}`;
    setPendingAction(actionKey);
    try {
      await patientsApi.updateStatus(patient.id, { treatmentStatus, reason: 'Atualizado pelo workspace web' });
      onPatientsChange(
        data.patients.map((item) => (item.id === patient.id ? { ...item, treatmentStatus } : item)),
      );
      notify('Status de tratamento atualizado.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        onPatientsChange(
          data.patients.map((item) => (item.id === patient.id ? { ...item, treatmentStatus } : item)),
        );
        notify('Status atualizado localmente.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel alterar o status.', 'danger');
    } finally {
      setPendingAction(null);
    }
  }

  async function loadSessionsSummary(patient: Patient) {
    const actionKey = `summary:${patient.id}`;
    setPendingAction(actionKey);
    try {
      await patientsApi.sessionsSummary(patient.id);
      notify('Resumo de sessoes carregado.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        notify('Resumo de sessoes disponivel em modo local.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel carregar o resumo.', 'danger');
    } finally {
      setPendingAction(null);
    }
  }

  return (
    <div className="resource-layout patients-page">
      <Section
        title="Pacientes"
        description="Busca, cadastro e contexto operacional sem expor notas clinicas na lista."
        action={
          <Button
            type="button"
            onClick={() => {
              setSelected(null);
              setMode('create');
            }}
          >
            <UserPlus size={17} aria-hidden="true" />
            Novo paciente
          </Button>
        }
      >
        <div className="patients-summary" aria-label="Resumo de pacientes">
          <div>
            <strong>{data.patients.length}</strong>
            <span>pacientes cadastrados</span>
          </div>
          <div>
            <strong>{activeCount}</strong>
            <span>ativos</span>
          </div>
          <div>
            <strong>{waitingCount}</strong>
            <span>aguardando</span>
          </div>
          <div>
            <strong>{incompleteCount}</strong>
            <span>cadastros incompletos</span>
          </div>
        </div>

        <div className="patients-toolbar" aria-label="Busca e filtros de pacientes">
          <label className="input-with-icon patients-search">
            <Search aria-hidden="true" size={18} />
            <span className="sr-only">Buscar pacientes</span>
            <input
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Buscar por nome, telefone ou email"
            />
          </label>
          <div className="patients-filters" aria-label="Filtrar por status">
            {(['Todos', 'Ativo', 'Aguardando', 'Inativo'] as const).map((status) => (
              <button
                key={status}
                type="button"
                className={statusFilter === status ? 'filter-chip filter-chip--active' : 'filter-chip'}
                aria-pressed={statusFilter === status}
                onClick={() => setStatusFilter(status)}
              >
                {status}
              </button>
            ))}
          </div>
        </div>

        <p className="privacy-note">
          <ShieldAlert aria-hidden="true" size={16} /> Dados clinicos detalhados ficam no prontuario. Esta lista mostra
          apenas informacoes operacionais.
        </p>

        {hasNoPatients ? <PatientsEmptyState onCreate={() => setMode('create')} /> : null}
        {hasNoResults ? (
          <PatientsNoResults
            query={query}
            onClear={() => {
              setQuery('');
              setStatusFilter('Todos');
            }}
          />
        ) : null}
        {!hasNoPatients && !hasNoResults ? (
          <PatientList
            patients={filteredPatients}
            onOpen={(patient) => setSelected(patient)}
            onEdit={(patient) => {
              setSelected(patient);
              setMode('edit');
            }}
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
        actions={
          selected ? (
            <>
              <Button type="button" variant="secondary" disabled title="Atalho ainda nao conectado ao prontuario">
                <FileText size={16} aria-hidden="true" />
                Prontuario indisponivel
              </Button>
              <Button type="button" variant="secondary" disabled title="Atalho ainda nao conectado a agenda">
                <CalendarPlus size={16} aria-hidden="true" />
                Agenda indisponivel
              </Button>
              <Button
                type="button"
                variant="secondary"
                disabled={!selected.email || !selected.phone || pendingAction !== null}
                onClick={() => invitePatient(selected)}
              >
                <UserPlus size={16} aria-hidden="true" />
                {pendingAction === `invite:${selected.id}` ? 'Enviando convite...' : 'Enviar convite'}
              </Button>
              {!selected.email || !selected.phone ? (
                <p className="drawer-action-hint">Informe email e telefone para enviar convite.</p>
              ) : null}
              <Button
                type="button"
                variant="secondary"
                disabled={pendingAction !== null}
                onClick={() => loadSessionsSummary(selected)}
              >
                {pendingAction === `summary:${selected.id}` ? 'Carregando resumo...' : 'Resumo de sessoes'}
              </Button>
              <div className="drawer-action-group" aria-label="Status de tratamento">
                <span className="drawer-action-group__label">
                  Tratamento atual: {formatTreatment(selected.treatmentStatus)}
                </span>
                <div className="drawer-action-group__items">
                  <Button
                    type="button"
                    variant="secondary"
                    disabled={selected.treatmentStatus === 'active_treatment' || pendingAction !== null}
                    onClick={() => changeTreatmentStatus(selected, 'active_treatment')}
                  >
                    {pendingAction === `treatment:${selected.id}:active_treatment`
                      ? 'Atualizando...'
                      : 'Marcar em tratamento'}
                  </Button>
                  <Button
                    type="button"
                    variant="secondary"
                    disabled={selected.treatmentStatus === 'paused' || pendingAction !== null}
                    onClick={() => changeTreatmentStatus(selected, 'paused')}
                  >
                    {pendingAction === `treatment:${selected.id}:paused` ? 'Atualizando...' : 'Marcar pausa'}
                  </Button>
                </div>
              </div>
              <Button type="button" variant="secondary" onClick={() => setMode('edit')}>
                <Edit3 size={16} aria-hidden="true" />
                Editar cadastro
              </Button>
              <Button
                type="button"
                className="button--danger"
                disabled={pendingAction !== null}
                onClick={() => setInactiveTarget(selected)}
              >
                Inativar paciente
              </Button>
            </>
          ) : null
        }
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
        description={
          inactiveTarget
            ? `${getPatientName(inactiveTarget)} deixara de aparecer como paciente ativo. O historico clinico nao sera removido.`
            : ''
        }
        confirmLabel="Inativar paciente"
        isDanger
        isSubmitting={inactiveTarget ? pendingAction === `inactivate:${inactiveTarget.id}` : false}
        onClose={() => setInactiveTarget(null)}
        onConfirm={confirmInactivate}
      />
    </div>
  );
}

function PatientList({
  patients,
  onOpen,
  onEdit,
  onInactivate,
}: {
  patients: Patient[];
  onOpen: (patient: Patient) => void;
  onEdit: (patient: Patient) => void;
  onInactivate: (patient: Patient) => void;
}) {
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
              <td data-label="Paciente">
                <div className="person-cell">
                  <strong>{getPatientName(patient)}</strong>
                  <span>{patient.email || 'Email nao informado'}</span>
                </div>
              </td>
              <td data-label="Contato">{patient.phone || 'Nao informado'}</td>
              <td data-label="Tratamento">
                <Badge tone="info">{formatTreatment(patient.treatmentStatus)}</Badge>
              </td>
              <td data-label="Proxima sessao">{patient.nextSession || 'Sem sessao agendada'}</td>
              <td data-label="Status">
                <Badge tone={getStatusTone(patient.status)}>{patient.status}</Badge>
              </td>
              <td data-label="Acoes">
                <div className="row-actions">
                  <button
                    type="button"
                    className="table-action"
                    onClick={() => onOpen(patient)}
                    aria-label={`Abrir detalhes de ${getPatientName(patient)}`}
                  >
                    <Eye size={15} aria-hidden="true" />
                    <span>Abrir</span>
                  </button>
                  <button
                    type="button"
                    className="table-action"
                    onClick={() => onEdit(patient)}
                    aria-label={`Editar ${getPatientName(patient)}`}
                  >
                    <Edit3 size={15} aria-hidden="true" />
                    <span>Editar</span>
                  </button>
                  <button
                    type="button"
                    className="table-action table-action--danger"
                    onClick={() => onInactivate(patient)}
                    aria-label={`Inativar ${getPatientName(patient)}`}
                  >
                    <Trash2 size={15} aria-hidden="true" />
                    <span>Inativar</span>
                  </button>
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
      <Button type="button" onClick={onCreate}>
        Cadastrar paciente
      </Button>
    </div>
  );
}

function PatientsNoResults({ query, onClear }: { query: string; onClear: () => void }) {
  return (
    <div className="empty-state patients-state">
      <Search aria-hidden="true" size={34} />
      <h3>Nenhum paciente encontrado</h3>
      <p>
        {query
          ? `A busca por "${query}" nao encontrou cadastro correspondente.`
          : 'Os filtros atuais nao retornaram pacientes.'}
      </p>
      <Button type="button" variant="secondary" onClick={onClear}>
        Limpar busca
      </Button>
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
  if (status === 'active' || status === 'active_treatment') return 'Em tratamento';
  if (status === 'paused') return 'Pausa';
  if (status === 'discharged') return 'Alta';
  return 'Triagem';
}

function toPayload(patient: Patient): Patient {
  return {
    ...patient,
    name: patient.fullName || patient.name,
    fullName: patient.fullName || patient.name,
    status: patient.status,
  };
}
