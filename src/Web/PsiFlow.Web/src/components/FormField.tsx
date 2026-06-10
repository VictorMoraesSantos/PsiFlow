import { useId } from 'react';
import type { FormField as FormFieldConfig, LookupMap } from '../types';

type FormFieldProps<T extends Record<string, unknown>> = {
  field: FormFieldConfig<T>;
  value: unknown;
  lookups?: LookupMap;
  onChange: (name: keyof T & string, value: unknown) => void;
};

export function FormField<T extends Record<string, unknown>>({ field, value, lookups, onChange }: FormFieldProps<T>) {
  const reactId = useId();
  const id = `field-${field.name}-${reactId}`;
  const helperId = field.helperText ? `${id}-helper` : undefined;
  const describedBy = helperId;
  const lookupOptions = field.type === 'lookup' && field.lookupKey ? lookups?.[field.lookupKey] ?? [] : [];

  if (field.type === 'checkbox') {
    return (
      <label className="form-field form-field--checkbox" htmlFor={id}>
        <span>{field.label}{field.required ? <em aria-hidden="true">*</em> : null}</span>
        <input
          id={id}
          name={field.name}
          type="checkbox"
          required={field.required}
          checked={Boolean(value)}
          onChange={(event) => onChange(field.name, event.target.checked)}
          aria-describedby={describedBy}
        />
        {field.helperText ? <small id={helperId}>{field.helperText}</small> : null}
      </label>
    );
  }

  return (
    <label className="form-field" htmlFor={id}>
      <span>{field.label}{field.required ? <em aria-hidden="true">*</em> : null}</span>
      {field.type === 'select' ? (
        <select
          id={id}
          name={field.name}
          required={field.required}
          value={String(value ?? '')}
          onChange={(event) => onChange(field.name, event.target.value)}
          aria-describedby={describedBy}
        >
          <option value="">Selecione</option>
          {field.options?.map((option) => (
            <option key={String(option.value)} value={String(option.value)}>
              {option.label}
            </option>
          ))}
        </select>
      ) : field.type === 'lookup' ? (
        <select
          id={id}
          name={field.name}
          required={field.required}
          value={value === undefined || value === null ? '' : String(value)}
          onChange={(event) => {
            const raw = event.target.value;
            const match = lookupOptions.find((option) => String(option.value) === raw);
            const next = match ? match.value : raw;
            onChange(field.name, typeof next === 'number' ? next : Number.isFinite(Number(next)) && field.name !== 'tenantId' ? Number(next) : next);
          }}
          aria-describedby={describedBy}
        >
          <option value="">Selecione</option>
          {lookupOptions.map((option) => (
            <option key={String(option.value)} value={String(option.value)}>
              {option.label}
            </option>
          ))}
        </select>
      ) : field.type === 'textarea' ? (
        <textarea
          id={id}
          name={field.name}
          required={field.required}
          placeholder={field.placeholder}
          value={String(value ?? '')}
          rows={4}
          onChange={(event) => onChange(field.name, event.target.value)}
          aria-describedby={describedBy}
        />
      ) : (
        <input
          id={id}
          name={field.name}
          type={field.type}
          required={field.required}
          placeholder={field.placeholder}
          value={String(value ?? '')}
          onChange={(event) =>
            onChange(field.name, field.type === 'number' ? Number(event.target.value) : event.target.value)
          }
          aria-describedby={describedBy}
        />
      )}
      {field.helperText ? <small id={helperId}>{field.helperText}</small> : null}
    </label>
  );
}
