import { useEffect, useState } from 'react';
import type { FormField, LookupMap } from '../types';
import { Button } from './Button';
import { FormField as Field } from './FormField';
import { Modal } from './Modal';

type ResourceFormModalProps<T extends Record<string, unknown>> = {
  isOpen: boolean;
  title: string;
  description?: string;
  fields: Array<FormField<T>>;
  lookups?: LookupMap;
  initialValue: T;
  submitLabel: string;
  isSubmitting?: boolean;
  onClose: () => void;
  onSubmit: (value: T) => Promise<void> | void;
};

export function ResourceFormModal<T extends Record<string, unknown>>({ isOpen, title, description, fields, lookups, initialValue, submitLabel, isSubmitting, onClose, onSubmit }: ResourceFormModalProps<T>) {
  const [value, setValue] = useState<T>(initialValue);

  useEffect(() => {
    if (isOpen) setValue(initialValue);
  }, [initialValue, isOpen]);

  function setField(name: keyof T & string, nextValue: unknown) {
    setValue((current) => ({ ...current, [name]: nextValue }));
  }

  async function submit(event: React.FormEvent) {
    event.preventDefault();
    await onSubmit(value);
  }

  return (
    <Modal isOpen={isOpen} title={title} description={description} onClose={onClose}>
      <form className="resource-form" onSubmit={submit} noValidate={false}>
        <p className="resource-form__note">Campos marcados com * precisam estar completos antes de salvar.</p>
        <div className="form-grid">
          {fields.map((field) => (
            <Field
              key={field.name}
              field={field}
              value={value[field.name]}
              lookups={lookups}
              onChange={setField}
            />
          ))}
        </div>
        <div className="form-actions">
          <Button type="button" variant="ghost" onClick={onClose}>Cancelar</Button>
          <Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Salvando...' : submitLabel}</Button>
        </div>
      </form>
    </Modal>
  );
}
