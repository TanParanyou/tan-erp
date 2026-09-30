"use client";

import type { ImgHTMLAttributes, ReactNode } from "react";
import { useTranslations } from "next-intl";
import { useAuthenticatedFileUrl } from "@/hooks/useAuthenticatedFileUrl";

interface AuthenticatedFileImageProps
  extends Omit<ImgHTMLAttributes<HTMLImageElement>, "src"> {
  fileId: string;
  fallback?: ReactNode;
}

export function AuthenticatedFileImage({
  fileId,
  fallback,
  alt,
  className,
  ...props
}: AuthenticatedFileImageProps) {
  const t = useTranslations("opportunities");
  const { objectUrl, isLoading, isError } = useAuthenticatedFileUrl(fileId);

  if (isLoading || !objectUrl) {
    if (fallback !== undefined) return <>{fallback}</>;
    return (
      <div
        className={className}
        role={isError ? "alert" : "status"}
        aria-label={isError ? t("protectedImageLoadError") : t("protectedImageLoading")}
      />
    );
  }

  if (isError) return fallback !== undefined ? <>{fallback}</> : null;

  return <img src={objectUrl} alt={alt} className={className} {...props} />;
}
