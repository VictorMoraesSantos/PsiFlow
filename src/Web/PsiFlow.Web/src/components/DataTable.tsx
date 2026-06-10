import { EmptyState } from './EmptyState';

type DataTableProps<T> = {
  columns: Array<{ key: string; header: string; render: (item: T) => React.ReactNode }>;
  items: T[];
  getRowKey: (item: T) => string | number;
  caption: string;
  emptyTitle: string;
  emptyDescription: string;
};

export function DataTable<T>({ columns, items, getRowKey, caption, emptyTitle, emptyDescription }: DataTableProps<T>) {
  if (items.length === 0) {
    return <EmptyState title={emptyTitle} description={emptyDescription} />;
  }

  return (
    <div className="table-wrap">
      <table>
        <caption>{caption}</caption>
        <thead>
          <tr>
            {columns.map((column) => (
              <th key={column.key} scope="col">{column.header}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={getRowKey(item)}>
              {columns.map((column) => (
                <td key={column.key}>{column.render(item)}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
