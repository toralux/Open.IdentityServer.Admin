import { AlertTriangle, ShieldX } from "lucide-react";
import { useTranslation } from "react-i18next";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/Card/Card";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { getStatusCode } from "@/helpers/ErrorHelper";

type HomeQueryErrorStateProps = {
  error: unknown;
  cardTitle?: string;
  cardDescription?: string;
  compact?: boolean;
};

const HomeQueryErrorState = ({
  error,
  cardTitle,
  cardDescription,
  compact = false,
}: HomeQueryErrorStateProps) => {
  const { t } = useTranslation();
  const status = getStatusCode(error);
  const isForbidden = status === 403;
  const isUnauthorized = status === 401;
  const Icon = isForbidden ? ShieldX : AlertTriangle;

  const description = isUnauthorized
    ? t("Home.DashboardState.Unauthorized")
    : isForbidden
      ? t("Home.DashboardState.Forbidden")
      : t("Home.DashboardState.Generic");

  const alert = (
    <Alert variant={isForbidden ? "default" : "destructive"}>
      <Icon className="h-4 w-4" />
      <AlertTitle>{t("Home.DashboardState.Title")}</AlertTitle>
      <AlertDescription>{description}</AlertDescription>
    </Alert>
  );

  if (compact) {
    return alert;
  }

  return (
    <Card className="col-span-1 overflow-hidden">
      {(cardTitle || cardDescription) && (
        <CardHeader>
          {cardTitle && <CardTitle>{cardTitle}</CardTitle>}
          {cardDescription && <CardDescription>{cardDescription}</CardDescription>}
        </CardHeader>
      )}
      <CardContent className="p-6">{alert}</CardContent>
    </Card>
  );
};

export default HomeQueryErrorState;
