import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/Card/Card";
import AuditLogs from "./AuditLogs";
import { getDashboardIdentityServerData } from "@/services/DashboardService";
import { useQuery } from "@tanstack/react-query";
import { useTranslation } from "react-i18next";
import ConfigurationIssuesSummary from "./ConfigurationIssuesSummary";
import { OverviewLeft } from "./OverviewLeft";
import Loading from "@/components/Loading/Loading";
import { queryKeys, queryWithoutCache } from "@/services/QueryKeys";
import HomeQueryErrorState from "./HomeQueryErrorState";

const DashboardCharts = () => {
  const { t } = useTranslation();

  const dashboardIdentityServer = useQuery({
    queryKey: [queryKeys.dashboard],
    queryFn: () => getDashboardIdentityServerData(30),
    ...queryWithoutCache,
  });

  return (
    <div>
      <div
        id="charts-stats"
        className="grid gap-4 md:grid-cols-1 lg:grid-cols-1x"
      >
        {dashboardIdentityServer.isLoading ? (
          <Loading />
        ) : dashboardIdentityServer.isError ? (
          <HomeQueryErrorState
            error={dashboardIdentityServer.error}
            cardTitle={t("Home.Overview")}
            cardDescription={t("Home.OverviewDescription")}
          />
        ) : (
          <OverviewLeft
            data={dashboardIdentityServer.data?.identityServerDataChart ?? []}
          />
        )}
        <ConfigurationIssuesSummary />
        <Card className="col-span-1">
          <CardHeader>
            <CardTitle>{t("Home.AuditLogs")}</CardTitle>
            <CardDescription>{t("Home.AuditLogsDescription")}</CardDescription>
          </CardHeader>
          <CardContent className="pl-2">
            {dashboardIdentityServer.isLoading ? (
              <Loading />
            ) : dashboardIdentityServer.isError ? (
              <HomeQueryErrorState
                error={dashboardIdentityServer.error}
                compact
              />
            ) : (
              <AuditLogs
                data={dashboardIdentityServer.data?.auditLogsData ?? []}
              />
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default DashboardCharts;
