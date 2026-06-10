type MetricTileProps = {
  label: string;
  value: string;
  detail: string;
  icon?: React.ReactNode;
  tone?: 'default' | 'accent';
};

export function MetricTile({ label, value, detail, icon, tone = 'default' }: MetricTileProps) {
  return (
    <div className={tone === 'accent' ? 'metric-tile metric-tile--accent' : 'metric-tile'}>
      {icon ? <span className="metric-tile__icon" aria-hidden="true">{icon}</span> : null}
      <span className="metric-tile__label">{label}</span>
      <strong className="metric-tile__value">{value}</strong>
      <p className="metric-tile__detail">{detail}</p>
    </div>
  );
}
