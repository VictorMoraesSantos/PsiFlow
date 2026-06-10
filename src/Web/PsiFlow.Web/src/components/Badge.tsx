import type { StatusTone } from '../types';

type BadgeProps = {
  children: React.ReactNode;
  tone?: StatusTone;
};

export function Badge({ children, tone = 'neutral' }: BadgeProps) {
  return <span className={`badge badge--${tone}`}>{children}</span>;
}
