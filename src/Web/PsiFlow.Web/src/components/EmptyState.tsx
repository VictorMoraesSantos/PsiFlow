import { FileSearch } from 'lucide-react';

type EmptyStateProps = {
  title: string;
  description: string;
};

export function EmptyState({ title, description }: EmptyStateProps) {
  return (
    <div className="empty-state">
      <FileSearch aria-hidden="true" size={28} />
      <h3>{title}</h3>
      <p>{description}</p>
    </div>
  );
}
