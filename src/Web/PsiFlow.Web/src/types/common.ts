export type StatusTone = 'neutral' | 'success' | 'warning' | 'danger' | 'info';
export type Id = string | number;

export type FieldType = 'text' | 'email' | 'tel' | 'number' | 'date' | 'datetime-local' | 'select' | 'lookup' | 'textarea' | 'checkbox';

export type LookupOption = { label: string; value: string | number };

export type FormField<T> = {
  name: keyof T & string;
  label: string;
  type: FieldType;
  required?: boolean;
  placeholder?: string;
  options?: Array<{ label: string; value: string | number | boolean }>;
  lookupKey?: string;
  helperText?: string;
};

export type LookupMap = Record<string, LookupOption[]>;
