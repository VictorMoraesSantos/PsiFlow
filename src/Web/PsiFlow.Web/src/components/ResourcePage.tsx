import { Edit3, Eye, Trash2 } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { isLocalFallbackStatus } from '../services/http';
import type { FormField, Id, LookupMap } from '../types';
import { Badge } from './Badge';
import { Button } from './Button';
import { ConfirmDialog } from './ConfirmDialog';
import { DataTable } from './DataTable';
import { DetailDrawer } from './DetailDrawer';
import { ResourceFormModal } from './ResourceFormModal';
import { Section } from './Section';
import { useToast } from './Toast';

type Column<T> = {
  key: string;
  header: string;
  render: (item: T) => React.ReactNode;
};

type ResourceAction<T> = {
  label: string;
  tone?: 'neutral' | 'danger';
  run: (item: T) => Promise<unknown> | unknown;
  successMessage?: string;
};

export type ResourceCrud<T> = {
  create: (payload: T) => Promise<T>;
  update: (id: Id, payload: T) => Promise<T>;
  remove: (id: Id) => Promise<void>;
};

type ResourcePageProps<T extends Record<string, unknown>> = {
  title: string;
  description: string;
  createLabel: string;
  items: T[];
  crud: ResourceCrud<T>;
  columns: Array<Column<T>>;
  fields: Array<FormField<T>>;
  lookups?: LookupMap;
  emptyValue: T;
  toCreatePayload: (value: T) => T;
  toUpdatePayload: (value: T) => T;
  getId: (item: T) => Id;
  getTitle: (item: T) => string;
  detailFields: Array<{ label: string; value: (item: T) => React.ReactNode }>;
  onItemsChange: (items: T[]) => void;
  actions?: Array<ResourceAction<T>>;
  summaryLabel?: (count: number) => string;
  summaryDescription?: (count: number) => string;
  updatePolicyLabel?: string;
  emptyTitle?: string;
  emptyDescription?: string;
  modalDescription?: string;
  createSubmitLabel?: string;
  editSubmitLabel?: string;
  detailEditLabel?: string;
  moreActionsLabel?: string;
  moreActionsHint?: string;
};

export function ResourcePage<T extends Record<string, unknown>>({
  title,
  description,
  createLabel,
  items,
  crud,
  columns,
  fields,
  lookups,
  emptyValue,
  toCreatePayload,
  toUpdatePayload,
  getId,
  getTitle,
  detailFields,
  onItemsChange,
  actions = [],
  summaryLabel,
  summaryDescription,
  updatePolicyLabel = 'Atualiza ao salvar',
  emptyTitle,
  emptyDescription,
  modalDescription = 'Revise os campos com calma. O workspace atualiza a lista depois que a alteracao for salva.',
  createSubmitLabel = 'Criar registro',
  editSubmitLabel = 'Salvar alteracoes',
  detailEditLabel = 'Editar registro',
  moreActionsLabel = 'Mais ações',
  moreActionsHint,
}: ResourcePageProps<T>) {
  const { notify } = useToast();
  const [mode, setMode] = useState<'create' | 'edit' | null>(null);
  const [selected, setSelected] = useState<T | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<T | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showMoreActions, setShowMoreActions] = useState(false);

  const formValue = mode === 'edit' && selected ? selected : emptyValue;
  const tableColumns = useMemo(
    () => [
      ...columns,
      {
        key: 'actions',
        header: 'Atalhos',
        render: (item: T) => (
          <ResourceRowActions
            title={getTitle(item)}
            onOpen={() => setSelected(item)}
            onEdit={() => {
              setSelected(item);
              setMode('edit');
            }}
            onDelete={() => setDeleteTarget(item)}
          />
        ),
      },
    ],
    [columns, getTitle],
  );

  useEffect(() => {
    setShowMoreActions(false);
  }, [selected]);

  async function submit(value: T) {
    setIsSubmitting(true);
    try {
      if (mode === 'create') {
        const created = await crud.create(toCreatePayload(value));
        onItemsChange([created, ...items]);
        notify(`${title}: registro criado.`);
      }
      if (mode === 'edit') {
        const updated = await crud.update(getId(value), toUpdatePayload(value));
        onItemsChange(items.map((item) => (getId(item) === getId(value) ? updated : item)));
        notify(`${title}: registro atualizado.`);
      }
      setMode(null);
      setSelected(null);
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        const optimistic = mode === 'create' ? { ...toCreatePayload(value), id: Date.now() } as T : toUpdatePayload(value);
        if (mode === 'create') onItemsChange([optimistic, ...items]);
        if (mode === 'edit') onItemsChange(items.map((item) => (getId(item) === getId(value) ? optimistic : item)));
        notify(`${title}: registro salvo em modo local.`, 'info');
        setMode(null);
        setSelected(null);
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel salvar.', 'danger');
    } finally {
      setIsSubmitting(false);
    }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    const id = getId(deleteTarget);
    try {
      await crud.remove(id);
      onItemsChange(items.filter((item) => getId(item) !== id));
      notify(`${title}: registro removido.`);
      setDeleteTarget(null);
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        onItemsChange(items.filter((item) => getId(item) !== id));
        notify(`${title}: registro removido em modo local.`, 'info');
        setDeleteTarget(null);
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel excluir.', 'danger');
    }
  }

  async function runAction(action: ResourceAction<T>, item: T) {
    try {
      await action.run(item);
      notify(action.successMessage ?? `${action.label}: acao concluida.`, action.tone === 'danger' ? 'info' : 'success');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        notify(`${action.label}: acao registrada localmente.`, 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel executar a acao.', 'danger');
    }
  }

  return (
    <div className="resource-layout">
      <Section
        title={title}
        description={description}
        action={
          <Button
            type="button"
            onClick={() => {
              setSelected(null);
              setMode('create');
            }}
          >
            {createLabel}
          </Button>
        }
      >
        <ResourceToolbar
          title={title}
          count={items.length}
          summaryLabel={summaryLabel}
          summaryDescription={summaryDescription}
          updatePolicyLabel={updatePolicyLabel}
        />
        <DataTable
          items={items}
          getRowKey={getId}
          caption={`${title}: registros operacionais`}
          emptyTitle={emptyTitle ?? `Nenhum registro em ${title.toLowerCase()}`}
          emptyDescription={
            emptyDescription ?? 'Quando houver dados neste fluxo, eles aparecem aqui com atalhos de revisao, edicao e fechamento.'
          }
          columns={tableColumns}
        />
      </Section>

      <DetailDrawer
        item={selected && mode === null ? selected : null}
        title={selected ? getTitle(selected) : title}
        fields={detailFields}
        onClose={() => setSelected(null)}
        actions={
          selected ? (
            <ResourceDrawerActions
              item={selected}
              actions={actions}
              editLabel={detailEditLabel}
              moreActionsLabel={moreActionsLabel}
              moreActionsHint={moreActionsHint}
              showMoreActions={showMoreActions}
              onEdit={() => setMode('edit')}
              onToggleMoreActions={() => setShowMoreActions((value) => !value)}
              onRunAction={runAction}
            />
          ) : null
        }
      />

      <ResourceFormModal
        isOpen={mode !== null}
        title={mode === 'create' ? createLabel : `Editar ${selected ? getTitle(selected) : title}`}
        description={modalDescription}
        fields={fields}
        lookups={lookups}
        initialValue={formValue}
        submitLabel={mode === 'create' ? createSubmitLabel : editSubmitLabel}
        isSubmitting={isSubmitting}
        onClose={() => setMode(null)}
        onSubmit={submit}
      />

      <ConfirmDialog
        isOpen={deleteTarget !== null}
        title="Excluir registro"
        description={
          deleteTarget
            ? `Esta acao remove ${getTitle(deleteTarget)} da lista. Confirme apenas se nao houver trabalho clinico pendente associado.`
            : ''
        }
        confirmLabel="Excluir registro"
        isDanger
        onClose={() => setDeleteTarget(null)}
        onConfirm={confirmDelete}
      />
    </div>
  );
}

function ResourceToolbar({
  title,
  count,
  summaryLabel,
  summaryDescription,
  updatePolicyLabel,
}: {
  title: string;
  count: number;
  summaryLabel?: (count: number) => string;
  summaryDescription?: (count: number) => string;
  updatePolicyLabel: string;
}) {
  const label = summaryLabel ? summaryLabel(count) : `${count} ${count === 1 ? 'registro' : 'registros'}`;
  const description = summaryDescription
    ? summaryDescription(count)
    : count === 0
    ? 'Nenhum item para revisar agora.'
    : 'Lista operacional com dados de trabalho do consultorio.';

  return (
    <div className="resource-toolbar" aria-label={`Resumo de ${title}`}>
      <div>
        <strong>{label}</strong>
        <span>{description}</span>
      </div>
      <div className="resource-toolbar__meta" aria-label="Politica de atualizacao">
        {updatePolicyLabel}
      </div>
    </div>
  );
}

function ResourceRowActions({
  title,
  onOpen,
  onEdit,
  onDelete,
}: {
  title: string;
  onOpen: () => void;
  onEdit: () => void;
  onDelete: () => void;
}) {
  return (
    <div className="row-actions">
      <button type="button" className="table-action" onClick={onOpen} aria-label={`Abrir detalhes de ${title}`}>
        <Eye size={15} aria-hidden="true" />
        <span>Abrir</span>
      </button>
      <button type="button" className="table-action" onClick={onEdit} aria-label={`Editar ${title}`}>
        <Edit3 size={15} aria-hidden="true" />
        <span>Editar</span>
      </button>
      <button
        type="button"
        className="table-action table-action--danger"
        onClick={onDelete}
        aria-label={`Excluir ${title}`}
      >
        <Trash2 size={15} aria-hidden="true" />
        <span>Excluir</span>
      </button>
    </div>
  );
}

function ResourceDrawerActions<T extends Record<string, unknown>>({
  item,
  actions,
  editLabel,
  moreActionsLabel,
  moreActionsHint,
  showMoreActions,
  onEdit,
  onToggleMoreActions,
  onRunAction,
}: {
  item: T;
  actions: Array<ResourceAction<T>>;
  editLabel: string;
  moreActionsLabel: string;
  moreActionsHint?: string;
  showMoreActions: boolean;
  onEdit: () => void;
  onToggleMoreActions: () => void;
  onRunAction: (action: ResourceAction<T>, item: T) => void;
}) {
  const primaryAction = actions[0];
  const secondaryActions = actions.slice(1);

  return (
    <>
      <Button type="button" variant="secondary" onClick={onEdit}>
        {editLabel}
      </Button>
      {primaryAction ? <ActionButton action={primaryAction} item={item} onRunAction={onRunAction} /> : null}
      {secondaryActions.length > 0 ? (
        <div className="drawer-action-group">
          <button
            type="button"
            className="drawer-action-group__toggle"
            onClick={onToggleMoreActions}
            aria-expanded={showMoreActions}
          >
            {moreActionsLabel}
          </button>
          {showMoreActions ? (
            <div className="drawer-action-group__items">
              {moreActionsHint ? <p className="drawer-action-hint">{moreActionsHint}</p> : null}
              {secondaryActions.map((action) => (
                <ActionButton key={action.label} action={action} item={item} onRunAction={onRunAction} />
              ))}
            </div>
          ) : null}
        </div>
      ) : null}
    </>
  );
}

function ActionButton<T extends Record<string, unknown>>({
  action,
  item,
  onRunAction,
}: {
  action: ResourceAction<T>;
  item: T;
  onRunAction: (action: ResourceAction<T>, item: T) => void;
}) {
  return (
    <Button
      type="button"
      variant={action.tone === 'danger' ? 'primary' : 'secondary'}
      className={action.tone === 'danger' ? 'button--danger' : ''}
      onClick={() => onRunAction(action, item)}
    >
      {action.label}
    </Button>
  );
}

export function statusBadge(status: string) {
  const normalized = status.toLowerCase();
  const tone = normalized.includes('ativo') ||
    normalized.includes('confirm') ||
    normalized.includes('pago') ||
    normalized.includes('assinado') ||
    normalized.includes('final')
    ? 'success'
    : normalized.includes('cancel') || normalized.includes('alto') || normalized.includes('revis')
    ? 'danger'
    : normalized.includes('pend') || normalized.includes('rascunho') || normalized.includes('andamento')
    ? 'warning'
    : 'info';
  return <Badge tone={tone}>{status}</Badge>;
}
