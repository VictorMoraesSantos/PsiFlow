import type { Metadata, Viewport } from 'next';
import { AppProvider } from '../state/AppContext';
import '../styles/global.css';

export const metadata: Metadata = {
  title: 'PsiFlow',
  description: 'PsiFlow, workspace clinico para psicologia.',
};

export const viewport: Viewport = {
  themeColor: '#1f3427',
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="pt-BR">
      <body>
        <AppProvider>{children}</AppProvider>
      </body>
    </html>
  );
}
