import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "./QueryKeys";
import ApiHelper from "@/helpers/ApiHelper";
import { client } from "@skoruba/duende.identityserver.admin.api.client";

export const useApplicationInformation = (options?: {
  enabled?: boolean;
  staleTime?: number;
  gcTime?: number;
}) =>
  useQuery({
    queryKey: [queryKeys.applicationInfo],
    queryFn: async () => {
      const configClient = new client.InfoClient(ApiHelper.getApiBaseUrl());

      const applicationName = await configClient.getApplicationName();
      const applicationVersion = await configClient.getApplicationVersion();

      return {
        applicationName,
        applicationVersion,
      };
    },
    ...options,
  });
