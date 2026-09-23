import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { t } from "i18next";
import { useTranslation } from "react-i18next";
import { Form } from "@/components/ui/form";
import { FormRow } from "@/components/FormRow/FormRow";
import { Button } from "@/components/ui/button";
import { CardWrapper } from "@/components/CardWrapper/CardWrapper";
import { KeyRound } from "lucide-react";
import { toast } from "@/components/ui/use-toast";
import { useChangeUserPassword } from "@/services/UserServices";
import Hoorey from "@/components/Hoorey/Hoorey";

const MIN_PASSWORD_LENGTH = 6;

const changePasswordSchema = z
  .object({
    password: z
      .string()
      .min(1, t("Validation.PasswordRequired"))
      .min(
        MIN_PASSWORD_LENGTH,
        t("Validation.PasswordMinLength", { min: MIN_PASSWORD_LENGTH }),
      ),
    confirmPassword: z.string().min(1, t("Validation.PasswordRequired")),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: t("Validation.PasswordsMustMatch"),
    path: ["confirmPassword"],
  });

type ChangePasswordFormData = z.infer<typeof changePasswordSchema>;

type Props = {
  userId: string;
};

const ChangePasswordTab: React.FC<Props> = ({ userId }) => {
  const { t } = useTranslation();
  const changePasswordMutation = useChangeUserPassword();

  const form = useForm<ChangePasswordFormData>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      password: "",
      confirmPassword: "",
    },
  });

  const onSubmit = (data: ChangePasswordFormData) => {
    changePasswordMutation.mutate(
      {
        userId,
        password: data.password,
        confirmPassword: data.confirmPassword,
      },
      {
        onSuccess: () => {
          form.reset();
          toast({
            title: <Hoorey />,
            description: t("User.ChangePassword.Success"),
          });
        },
      },
    );
  };

  return (
    <CardWrapper
      title={t("User.ChangePassword.Title")}
      description={t("User.ChangePassword.Description")}
      icon={KeyRound}
    >
      <Form {...form}>
        <div className="space-y-4">
          <FormRow
            name="password"
            label={t("User.ChangePassword.NewPassword_Label")}
            description={t("User.ChangePassword.NewPassword_Info")}
            type="input"
            inputType="password"
            required
          />
          <FormRow
            name="confirmPassword"
            label={t("User.ChangePassword.ConfirmPassword_Label")}
            description={t("User.ChangePassword.ConfirmPassword_Info")}
            type="input"
            inputType="password"
            required
          />
          <div className="flex justify-start pt-2">
            <Button
              type="button"
              onClick={form.handleSubmit(onSubmit)}
              disabled={changePasswordMutation.isPending}
            >
              {t("User.ChangePassword.Submit")}
            </Button>
          </div>
        </div>
      </Form>
    </CardWrapper>
  );
};

export default ChangePasswordTab;
