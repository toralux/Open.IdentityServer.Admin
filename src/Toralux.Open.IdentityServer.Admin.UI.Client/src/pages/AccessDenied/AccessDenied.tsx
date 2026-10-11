import { useEffect } from "react";
import { ShieldX } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { HomeUrl } from "@/routing/Urls";
import { Button } from "@/components/ui/button";

const AccessDenied = () => {
  const { t } = useTranslation();

  useEffect(() => {
    document.title = t("Pages.AccessDenied.Title");
  }, [t]);

  return (
    <div className="min-h-[60vh] flex items-center justify-center p-8">
      <div className="text-center space-y-6 max-w-md">
        <div className="flex justify-center">
          <ShieldX className="h-20 w-20 text-destructive opacity-80" />
        </div>
        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight">
            {t("Pages.AccessDenied.Title")}
          </h1>
          <p className="text-muted-foreground text-base">
            {t("Pages.AccessDenied.Description")}
          </p>
        </div>
        <Button asChild>
          <Link to={HomeUrl}>{t("Pages.AccessDenied.GoHome")}</Link>
        </Button>
      </div>
    </div>
  );
};

export default AccessDenied;
