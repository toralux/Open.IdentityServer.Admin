import { useEffect, useState } from "react";
import { KeyRound } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import Loading from "@/components/Loading/Loading";
import AuthHelper from "@/helpers/AuthHelper";
import { HomeUrl } from "@/routing/Urls";

const Unauthorized = () => {
  const { t } = useTranslation();
  const [isSigningIn, setIsSigningIn] = useState(false);

  useEffect(() => {
    document.title = t("Pages.Unauthorized.Title");
  }, [t]);

  const handleSignInAgain = () => {
    setIsSigningIn(true);
    AuthHelper.redirectToLoginUrl(HomeUrl);
  };

  return (
    <div className="min-h-[60vh] flex items-center justify-center p-8">
      <div className="text-center space-y-6 max-w-md">
        <div className="flex justify-center">
          <KeyRound className="h-20 w-20 text-destructive opacity-80" />
        </div>
        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight">
            {t("Pages.Unauthorized.Title")}
          </h1>
          <p className="text-muted-foreground text-base">
            {t("Pages.Unauthorized.Description")}
          </p>
        </div>
        <Button
          onClick={handleSignInAgain}
          disabled={isSigningIn}
          className="min-w-40 gap-2"
        >
          {isSigningIn && <Loading size="sm" />}
          {t("Pages.Unauthorized.SignInAgain")}
        </Button>
      </div>
    </div>
  );
};

export default Unauthorized;
