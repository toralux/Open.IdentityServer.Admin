import "@/globals.css";
import { ThemeProvider } from "@/components/ThemeProvider/ThemeProvider";
import { Toaster } from "../ui/toaster";

interface MinimalLayoutProps {
  children: React.ReactNode;
}

export default function MinimalLayout({ children }: MinimalLayoutProps) {
  return (
    <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
      <div className="flex min-h-screen flex-col">
        <main className="flex-1">{children}</main>
      </div>
      <Toaster />
    </ThemeProvider>
  );
}
