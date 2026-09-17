import { useMemo } from "react";
import { AlertTriangle } from "lucide-react";
import { useTranslation } from "react-i18next";
import { FieldErrors, useFormContext } from "react-hook-form";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";

type ValidationSummaryItem = {
  fieldPath: string;
  message: string;
};

type FormValidationSummaryProps = {
  fieldLabels?: Record<string, string>;
};

const normalizeFieldPath = (fieldPath: string) =>
  fieldPath
    .split(".")
    .filter((segment) => !/^\d+$/.test(segment))
    .join(".");

const humanizeFieldPath = (fieldPath: string) =>
  fieldPath
    .split(".")
    .map((segment) => segment.replace(/([a-z0-9])([A-Z])/g, "$1 $2"))
    .map((segment) => segment.charAt(0).toUpperCase() + segment.slice(1))
    .join(" / ");

const getFieldLabel = (
  fieldPath: string,
  fieldLabels?: Record<string, string>,
) => {
  const normalizedFieldPath = normalizeFieldPath(fieldPath);
  const pathSegments = normalizedFieldPath.split(".");
  const arrayIndex = fieldPath
    .split(".")
    .find((segment) => /^\d+$/.test(segment));

  const fieldLabel =
    fieldLabels?.[normalizedFieldPath] ??
    fieldLabels?.[pathSegments[0]] ??
    humanizeFieldPath(normalizedFieldPath);

  if (arrayIndex == null) {
    return fieldLabel;
  }

  return `${fieldLabel} #${Number(arrayIndex) + 1}`;
};

const collectErrorMessages = (errors: FieldErrors): ValidationSummaryItem[] => {
  const messages: ValidationSummaryItem[] = [];

  const visit = (value: unknown, path: string[]) => {
    if (value == null) {
      return;
    }

    if (Array.isArray(value)) {
      value.forEach((item, index) => visit(item, [...path, index.toString()]));
      return;
    }

    if (typeof value !== "object") {
      return;
    }

    if (
      "message" in value &&
      typeof value.message === "string" &&
      value.message.trim().length > 0
    ) {
      messages.push({
        fieldPath: path.join("."),
        message: value.message.trim(),
      });
    }

    Object.entries(value).forEach(([key, childValue]) => {
      if (key === "message" || key === "type" || key === "ref") {
        return;
      }

      visit(childValue, [...path, key]);
    });
  };

  Object.entries(errors).forEach(([key, value]) => {
    visit(value, [key]);
  });

  return messages.filter(
    (item, index, items) =>
      items.findIndex(
        (otherItem) =>
          otherItem.fieldPath === item.fieldPath &&
          otherItem.message === item.message,
      ) === index,
  );
};

const FormValidationSummary = ({
  fieldLabels,
}: FormValidationSummaryProps) => {
  const { t } = useTranslation();
  const { formState } = useFormContext();

  const validationSummaryItems = useMemo(
    () => collectErrorMessages(formState.errors),
    [formState.errors],
  );

  if (formState.submitCount === 0 || validationSummaryItems.length === 0) {
    return null;
  }

  return (
    <Alert variant="destructive" className="mb-6">
      <AlertTriangle className="h-4 w-4" />
      <AlertTitle>{t("Components.ValidationSummary.Title")}</AlertTitle>
      <AlertDescription>
        <ul className="mt-2 list-disc space-y-1 pl-5">
          {validationSummaryItems.map(({ fieldPath, message }, index) => (
            <li key={`${fieldPath}-${message}-${index}`}>
              <span className="font-medium">
                {getFieldLabel(fieldPath, fieldLabels)}:
              </span>{" "}
              {message}
            </li>
          ))}
        </ul>
      </AlertDescription>
    </Alert>
  );
};

export default FormValidationSummary;
