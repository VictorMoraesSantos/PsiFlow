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
  const label = <span>{field.label}{field.required ? <em aria-hidden="true">*</em> : null}</span>;
  const helper = field.helperText ? <small id={helperId}>{field.helperText}</small> : null;

  if (field.type === 'checkbox') {
    return (
      <label className="form-field form-field--checkbox" htmlFor={id}>
        {label}
        <input
          id={id}
          name={field.name}
          type="checkbox"
          required={field.required}
          checked={Boolean(value)}
          onChange={(event) => onChange(field.name, event.target.checked)}
          aria-describedby={describedBy}
        />
        {helper}
      </label>
    );
  }

  return (
    <label className="form-field" htmlFor={id}>
      {label}
      <FieldControl id={id} field={field} value={value} lookupOptions={lookupOptions} describedBy={describedBy} onChange={onChange} />
      {helper}
    </label>
  );
}

function FieldControl<T extends Record<string, unknown>>({ id, field, value, lookupOptions, describedBy, onChange }: {
  id: string;
  field: FormFieldConfig<T>;
  value: unknown;
  lookupOptions: Array<{ label: string; value: string | number }>;
  describedBy?: string;
  onChange: (name: keyof T & string, value: unknown) => void;
}) {
  if (field.type === 'select') {
    return <SelectField id={id} field={field} value={String(value ?? '')} describedBy={describedBy} onChange={onChange} />;
  }

  if (field.type === 'lookup') {
    return <LookupField id={id} field={field} value={value} options={lookupOptions} describedBy={describedBy} onChange={onChange} />;
  }

  if (field.type === 'textarea') {
    return <textarea id={id} name={field.name} required={field.required} placeholder={field.placeholder} value={String(value ?? '')} rows={4} onChange={(event) => onChange(field.name, event.target.value)} aria-describedby={describedBy} />;
  }

  return <input id={id} name={field.name} type={field.type} required={field.required} placeholder={field.placeholder} value={String(value ?? '')} onChange={(event) => onChange(field.name, field.type === 'number' ? Number(event.target.value) : event.target.value)} aria-describedby={describedBy} />;
}

function SelectField<T extends Record<string, unknown>>({ id, field, value, describedBy, onChange }: {
  id: string;
  field: FormFieldConfig<T>;
  value: string;
  describedBy?: string;
  onChange: (name: keyof T & string, value: unknown) => void;
}) {
  return (
    <select id={id} name={field.name} required={field.required} value={value} onChange={(event) => onChange(field.name, event.target.value)} aria-describedby={describedBy}>
      <option value="">Selecione</option>
      {field.options?.map((option) => <option key={String(option.value)} value={String(option.value)}>{option.label}</option>)}
    </select>
  );
}

function LookupField<T extends Record<string, unknown>>({ id, field, value, options, describedBy, onChange }: {
  id: string;
  field: FormFieldConfig<T>;
  value: unknown;
  options: Array<{ label: string; value: string | number }>;
  describedBy?: string;
  onChange: (name: keyof T & string, value: unknown) => void;
}) {
  return (
    <select id={id} name={field.name} required={field.required} value={value === undefined || value === null ? '' : String(value)} onChange={(event) => onChange(field.name, parseLookupValue(field.name, event.target.value, options))} aria-describedby={describedBy}>
      <option value="">Selecione</option>
      {options.map((option) => <option key={String(option.value)} value={String(option.value)}>{option.label}</option>)}
    </select>
  );
}

function parseLookupValue(fieldName: string, raw: string, options: Array<{ value: string | number }>) {
  const match = options.find((option) => String(option.value) === raw);
  const next = match ? match.value : raw;
  return typeof next === 'number' ? next : Number.isFinite(Number(next)) && fieldName !== 'tenantId' ? Number(next) : next;
}
