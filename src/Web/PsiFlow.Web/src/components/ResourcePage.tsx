import { Edit3, Eye, Trash2 } from 'lucide-react';
import { useMemo, useState } from 'react';
import type { ApiService } from '../services/api';
import { createResource, deleteResource, updateResource } from '../services/api';
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

type ResourcePageProps<T extends Record<string, unknown>> = {
  title: string;
  description: string;
  createLabel: string;
  items: T[];
  service: ApiService;
  path: string;
  columns: Array<Column<T>>;
  fields: Array<FormField<T>>;
  lookups?: LookupMap;
  emptyValue: T;
  toCreatePayload: (value: T) => unknown;
  toUpdatePayload: (value: T) => unknown;
  getId: (item: T) => Id;
  getTitle: (item: T) => string;
  detailFields: Array<{ label: string; value: (item: T) => React.ReactNode }>;
  onItemsChange: (items: T[]) => void;
  actions?: Array<ResourceAction<T>>;
};

export function ResourcePage<T extends Record<string, unknown>>({ title, description, createLabel, items, service, path, columns, fields, lookups, emptyValue, toCreatePayload, toUpdatePayload, getId, getTitle, detailFields, onItemsChange, actions = [] }: ResourcePageProps<T>) {
  const { notify } = useToast();
  const [mode, setMode] = useState<'create' | 'edit' | null>(null);
  const [selected, setSelected] = useState<T | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<T | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const formValue = mode === 'edit' && selected ? selected : emptyValue;
  const tableColumns = useMemo(() => [
    ...columns,
    {
      key: 'actions',
      header: 'Atalhos',
      render: (item: T) => (
        <div className="row-actions">
          <button type="button" className="table-action" onClick={() => setSelected(item)} aria-label={`Abrir detalhes de ${getTitle(item)}`}><Eye size={15} aria-hidden="true" /><span>Abrir</span></button>
          <button type="button" className="table-action" onClick={() => { setSelected(item); setMode('edit'); }} aria-label={`Editar ${getTitle(item)}`}><Edit3 size={15} aria-hidden="true" /><span>Editar</span></button>
          <button type="button" className="table-action table-action--danger" onClick={() => setDeleteTarget(item)} aria-label={`Excluir ${getTitle(item)}`}><Trash2 size={15} aria-hidden="true" /><span>Excluir</span></button>
        </div>
      ),
    },
  ], [actions.length, columns, getTitle]);

  async function submit(value: T) {
    setIsSubmitting(true);
    try {
      if (mode === 'create') {
        const fallback = { ...value, id: Date.now() } as T;
        const created = await createResource<T>(service, path, toCreatePayload(value), fallback);
        onItemsChange([created, ...items]);
        notify(`${title}: registro criado.`);
      }
      if (mode === 'edit') {
        const updated = await updateResource<T>(service, path, getId(value), toUpdatePayload(value), value);
        onItemsChange(items.map((item) => getId(item) === getId(value) ? updated : item));
        notify(`${title}: registro atualizado.`);
      }
      setMode(null);
      setSelected(null);
    } catch (error) {
      notify(error instanceof Error ? error.message : 'Nao foi possivel salvar.', 'danger');
    } finally {
      setIsSubmitting(false);
    }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    try {
      await deleteResource(service, path, getId(deleteTarget));
      onItemsChange(items.filter((item) => getId(item) !== getId(deleteTarget)));
      notify(`${title}: registro removido.`);
      setDeleteTarget(null);
    } catch (error) {
      notify(error instanceof Error ? error.message : 'Nao foi possivel excluir.', 'danger');
    }
  }

  async function runAction(action: ResourceAction<T>, item: T) {
    try {
      await action.run(item);
      notify(action.successMessage ?? `${action.label}: acao concluida.`, action.tone === 'danger' ? 'info' : 'success');
    } catch (error) {
      notify(error instanceof Error ? error.message : 'Nao foi possivel executar a acao.', 'danger');
    }
  }

  return (
    <div className="resource-layout">
      <Section title={title} description={description} action={<Button type="button" onClick={() => { setSelected(null); setMode('create'); }}>{createLabel}</Button>}>
        <div className="resource-toolbar" aria-label={`Resumo de ${title}`}>
          <div>
            <strong>{items.length} {items.length === 1 ? 'registro' : 'registros'}</strong>
            <span>{items.length === 0 ? 'Nenhum item para revisar agora.' : 'Lista operacional com dados de trabalho do consultorio.'}</span>
          </div>
          <div className="resource-toolbar__meta" aria-label="Politica de atualizacao">
            Atualiza ao salvar
          </div>
        </div>
        <DataTable
          items={items}
          getRowKey={getId}
          caption={`${title}: registros operacionais`}
          emptyTitle={`Nenhum registro em ${title.toLowerCase()}`}
          emptyDescription="Quando houver dados neste fluxo, eles aparecem aqui com atalhos de revisao, edicao e fechamento."
          columns={tableColumns}
        />
      </Section>

      <DetailDrawer
        item={selected && mode === null ? selected : null}
        title={selected ? getTitle(selected) : title}
        fields={detailFields}
        onClose={() => setSelected(null)}
        actions={selected ? (
          <>
            <Button type="button" variant="secondary" onClick={() => setMode('edit')}>Editar registro</Button>
            {actions.map((action) => (
              <Button key={action.label} type="button" variant={action.tone === 'danger' ? 'primary' : 'secondary'} className={action.tone === 'danger' ? 'button--danger' : ''} onClick={() => runAction(action, selected)}>{action.label}</Button>
            ))}
          </>
        ) : null}
      />

      <ResourceFormModal
        isOpen={mode !== null}
        title={mode === 'create' ? createLabel : `Editar ${selected ? getTitle(selected) : title}`}
        description="Revise os campos com calma. O workspace atualiza a lista depois que a alteracao for salva."
        fields={fields}
        lookups={lookups}
        initialValue={formValue}
        submitLabel={mode === 'create' ? 'Criar registro' : 'Salvar alteracoes'}
        isSubmitting={isSubmitting}
        onClose={() => setMode(null)}
        onSubmit={submit}
      />

      <ConfirmDialog
        isOpen={deleteTarget !== null}
        title="Excluir registro"
        description={deleteTarget ? `Esta acao remove ${getTitle(deleteTarget)} da lista. Confirme apenas se nao houver trabalho clinico pendente associado.` : ''}
        confirmLabel="Excluir registro"
        isDanger
        onClose={() => setDeleteTarget(null)}
        onConfirm={confirmDelete}
      />
    </div>
  );
}

export function statusBadge(status: string) {
  const normalized = status.toLowerCase();
  const tone = normalized.includes('ativo') || normalized.includes('confirm') || normalized.includes('pago') || normalized.includes('assinado') || normalized.includes('final')
    ? 'success'
    : normalized.includes('cancel') || normalized.includes('alto') || normalized.includes('revis')
      ? 'danger'
      : normalized.includes('pend') || normalized.includes('rascunho') || normalized.includes('andamento')
        ? 'warning'
        : 'info';
  return <Badge tone={tone}>{status}</Badge>;
}
