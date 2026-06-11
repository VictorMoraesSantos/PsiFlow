import { ChevronLeft, ChevronRight } from 'lucide-react';
import { useMemo, useState } from 'react';
import { Badge } from './Badge';
import { Button } from './Button';
import type { Appointment } from '../types';

type MonthCalendarProps = {
  appointments: Appointment[];
  title?: string;
  compact?: boolean;
  onCreateForDay?: (date: Date) => void;
};

const weekdayLabels = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'];

export function MonthCalendar({ appointments, title = 'Calendario do mes', compact = false, onCreateForDay }: MonthCalendarProps) {
  const [visibleMonth, setVisibleMonth] = useState(() => startOfMonth(new Date()));
  const [selectedDay, setSelectedDay] = useState(() => startOfDay(new Date()));

  const cells = useMemo(() => buildCalendarCells(visibleMonth), [visibleMonth]);
  const appointmentsByDay = useMemo(() => groupAppointmentsByDay(appointments), [appointments]);
  const selectedKey = dayKey(selectedDay);
  const selectedAppointments = appointmentsByDay.get(selectedKey) ?? [];

  function moveMonth(delta: number) {
    setVisibleMonth((current) => new Date(current.getFullYear(), current.getMonth() + delta, 1));
  }

  return (
    <div className={compact ? 'month-calendar month-calendar--compact' : 'month-calendar'}>
      <div className="month-calendar__header">
        <div>
          <h3>{title}</h3>
          <p>{visibleMonth.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })}</p>
        </div>
        <div className="month-calendar__nav" aria-label="Navegar meses">
          <button type="button" className="icon-button" onClick={() => moveMonth(-1)} aria-label="Mes anterior"><ChevronLeft size={18} aria-hidden="true" /></button>
          <button type="button" className="icon-button" onClick={() => moveMonth(1)} aria-label="Proximo mes"><ChevronRight size={18} aria-hidden="true" /></button>
        </div>
      </div>

      <div className="month-calendar__grid" aria-label="Dias do mes">
        {weekdayLabels.map((label) => <span className="month-calendar__weekday" key={label}>{label}</span>)}
        {cells.map((date) => {
          const key = dayKey(date);
          const dayAppointments = appointmentsByDay.get(key) ?? [];
          const isCurrentMonth = date.getMonth() === visibleMonth.getMonth();
          const isSelected = key === selectedKey;
          const isToday = key === dayKey(new Date());

          return (
            <button
              type="button"
              key={key}
              className={[
                'month-calendar__day',
                !isCurrentMonth ? 'month-calendar__day--muted' : '',
                isSelected ? 'month-calendar__day--selected' : '',
                isToday ? 'month-calendar__day--today' : '',
              ].filter(Boolean).join(' ')}
              onClick={() => setSelectedDay(startOfDay(date))}
              aria-pressed={isSelected}
            >
              <span>{date.getDate()}</span>
              {dayAppointments.length ? <strong>{dayAppointments.length}</strong> : null}
            </button>
          );
        })}
      </div>

      <div className="month-calendar__agenda" aria-live="polite">
        <div className="month-calendar__agenda-header">
          <strong>{selectedDay.toLocaleDateString('pt-BR', { weekday: 'long', day: '2-digit', month: '2-digit' })}</strong>
          <Badge tone={selectedAppointments.length ? 'info' : 'neutral'}>{selectedAppointments.length ? `${selectedAppointments.length} compromisso${selectedAppointments.length > 1 ? 's' : ''}` : 'Livre'}</Badge>
        </div>
        {selectedAppointments.length ? (
          <ul>
            {selectedAppointments.map((appointment) => (
              <li key={appointment.id}>
                <span>{appointment.time || formatTime(appointment.startsAt)}</span>
                <div>
                  <strong>{appointment.patientName || appointment.name || `Consulta ${appointment.id}`}</strong>
                  <small>{appointment.kind} · {appointment.status}</small>
                </div>
              </li>
            ))}
          </ul>
        ) : (
          <p>Nenhum compromisso neste dia.</p>
        )}
        {!compact && onCreateForDay ? <Button type="button" variant="secondary" onClick={() => onCreateForDay(selectedDay)}>Criar compromisso neste dia</Button> : null}
      </div>
    </div>
  );
}

function buildCalendarCells(month: Date) {
  const first = startOfMonth(month);
  const start = new Date(first);
  start.setDate(first.getDate() - first.getDay());
  return Array.from({ length: 42 }, (_, index) => {
    const date = new Date(start);
    date.setDate(start.getDate() + index);
    return date;
  });
}

function groupAppointmentsByDay(appointments: Appointment[]) {
  const map = new Map<string, Appointment[]>();
  for (const appointment of appointments) {
    const key = dayKey(parseAppointmentDate(appointment));
    const items = map.get(key) ?? [];
    items.push(appointment);
    items.sort((a, b) => parseAppointmentDate(a).getTime() - parseAppointmentDate(b).getTime());
    map.set(key, items);
  }
  return map;
}

function parseAppointmentDate(appointment: Appointment) {
  return appointment.startsAt ? new Date(appointment.startsAt) : new Date();
}

function startOfMonth(date: Date) {
  return new Date(date.getFullYear(), date.getMonth(), 1);
}

function startOfDay(date: Date) {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

function dayKey(date: Date) {
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
}

function formatTime(value?: string) {
  if (!value) return '--:--';
  return new Date(value).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
}
