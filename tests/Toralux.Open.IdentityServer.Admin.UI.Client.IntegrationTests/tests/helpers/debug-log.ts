export function createDebugLogger(prefix: string): (message: string) => void {
  const shouldLogSteps = process.env.E2E_DEBUG_LOGS === "1";

  return (message: string) => {
    if (shouldLogSteps) {
      console.log(`[${prefix}] ${message}`);
    }
  };
}
