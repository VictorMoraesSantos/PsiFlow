import { Button } from './Button';
import { Modal } from './Modal';

type ConfirmDialogProps = {
  isOpen: boolean;
  title: string;
  description: string;
  confirmLabel: string;
  isDanger?: boolean;
  onClose: () => void;
  onConfirm: () => Promise<void> | void;
};

export function ConfirmDialog({ isOpen, title, description, confirmLabel, isDanger, onClose, onConfirm }: ConfirmDialogProps) {
  return (
    <Modal isOpen={isOpen} title={title} description={description} onClose={onClose}>
      <div className="form-actions">
        <Button type="button" variant="secondary" onClick={onClose}>Cancelar</Button>
        <Button type="button" className={isDanger ? 'button--danger' : ''} onClick={onConfirm}>{confirmLabel}</Button>
      </div>
    </Modal>
  );
}
