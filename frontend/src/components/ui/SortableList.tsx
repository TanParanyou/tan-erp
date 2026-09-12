"use client";

import React from "react";
import { useSortableList } from "@/hooks/useSortableList";
import { IconGripVertical } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface SortableListProps<T> {
  items: T[];
  renderItem: (item: T, index: number) => React.ReactNode;
  keyExtractor: (item: T, index: number) => string;
  onMove?: (from: number, to: number) => void;
  onCommit?: () => void;
  className?: string;
  itemClassName?: string;
}

export function SortableList<T>({
  items,
  renderItem,
  keyExtractor,
  onMove,
  onCommit,
  className,
  itemClassName,
}: SortableListProps<T>) {
  const {
    draggedIndex,
    overIndex,
    handleDragStart,
    handleDragOver,
    handleDragEnd,
  } = useSortableList({ onMove, onCommit });

  return (
    <div className={cn("flex flex-col gap-2", className)}>
      {items.map((item: T, index: number) => {
        const isDragging = draggedIndex === index;
        const isOver = overIndex === index;

        return (
          <div
            key={keyExtractor(item, index)}
            draggable
            onDragStart={() => handleDragStart(index)}
            onDragOver={(e) => {
              handleDragOver(e, index);
            }}
            onDragEnd={handleDragEnd}
            className={cn(
              "flex items-center gap-3 border border-erp-border bg-erp-surface p-3 rounded-none transition-all",
              isDragging && "opacity-50 border-dashed border-erp-navy",
              isOver && "border-erp-navy shadow-sm",
              itemClassName
            )}
          >
            <div className="cursor-grab text-erp-text-muted hover:text-erp-text-main active:cursor-grabbing">
              <IconGripVertical size={16} />
            </div>
            <div className="flex-1">{renderItem(item, index)}</div>
          </div>
        );
      })}
    </div>
  );
}

SortableList.displayName = "SortableList";
