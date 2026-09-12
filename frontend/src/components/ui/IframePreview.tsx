"use client";

import React, { useState } from "react";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { cn } from "@/lib/utils/cn";

export interface IframePreviewProps {
  src: string;
  title?: string;
  className?: string;
  height?: string;
}

export function IframePreview({
  src,
  title = "Preview",
  className,
  height = "450px",
}: IframePreviewProps) {
  const [isLoading, setIsLoading] = useState(true);

  return (
    <div
      style={{ height }}
      className={cn(
        "relative w-full overflow-hidden border border-erp-border bg-erp-surface rounded-none shadow-xs",
        className
      )}
    >
      {isLoading && (
        <div className="absolute inset-0 flex items-center justify-center bg-erp-surface/80 z-10">
          <MonoSpinner />
        </div>
      )}
      <iframe
        src={src}
        title={title}
        onLoad={() => setIsLoading(false)}
        className="h-full w-full border-none rounded-none"
      />
    </div>
  );
}

IframePreview.displayName = "IframePreview";
