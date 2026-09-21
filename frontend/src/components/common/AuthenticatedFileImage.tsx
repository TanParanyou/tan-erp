"use client";

import type { ImgHTMLAttributes } from "react";
import { useTranslations } from "next-intl";
import { useAuthenticatedFileUrl } from "@/hooks/useAuthenticatedFileUrl";

interface AuthenticatedFileImageProps
  extends Omit<ImgHTMLAttributes<HTMLImageElement>, "src"> {
  fileId: string;
}

export function AuthenticatedFileImage({
  fileId,
  alt,
  className,
  ...props
}: AuthenticatedFileImageProps) {
  const t = useTranslations("opportunities");
  const { objectUrl, isLoading, isError } = useAuthenticatedFileUrl(fileId);

  if (isLoading || !objectUrl) {
    return (
      <div
        className={className}
        role={isError ? "alert" : "status"}
        aria-label={isError ? t("protectedImageLoadError") : t("protectedImageLoading")}
      />
    );
  }

  return <img src={objectUrl} alt={alt} className={className} {...props} />;
}
