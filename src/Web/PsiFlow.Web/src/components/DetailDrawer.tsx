import { X } from 'lucide-react';
import { useEffect, useRef } from 'react';
import { useFocusTrap } from '../hooks/useFocusTrap';
import { Button } from './Button';

type DetailDrawerProps<T extends Record<string, unknown>> = {
  item: T | null;
  title: string;
  fields: Array<{ label: string; value: (item: T) => React.ReactNode }>;
  actions?: React.ReactNode;
  onClose: () => void;
};

export function DetailDrawer<T extends Record<string, unknown>>({ item, title, fields, actions, onClose }: DetailDrawerProps<T>) {
  const containerRef = useRef<HTMLElement>(null);
  useFocusTrap(containerRef, item !== null, onClose);

  useEffect(() => {
    if (!item) return;
    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = previousOverflow;
    };
  }, [item]);

  if (!item) return null;

  return (
    <aside
      ref={containerRef}
      className="detail-drawer"
      role="dialog"
      aria-modal="true"
      aria-labelledby="detail-title"
    >
      <div className="detail-drawer__header">
        <h2 id="detail-title">{title}</h2>
        <button className="icon-button" type="button" aria-label="Fechar detalhes" onClick={onClose}>
          <X aria-hidden="true" size={18} />
        </button>
      </div>
      <dl>
        {fields.map((field) => (
          <div key={field.label}>
            <dt>{field.label}</dt>
            <dd>{field.value(item)}</dd>
          </div>
        ))}
      </dl>
      {actions ? <div className="detail-drawer__actions">{actions}</div> : null}
      <Button type="button" variant="secondary" onClick={onClose}>Fechar detalhes</Button>
    </aside>
  );
}
