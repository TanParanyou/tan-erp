"use client";

import React, { createContext, useContext, useState, useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import type { MembershipDto } from "@/lib/permissions/can";

interface SelectedMembershipContextType {
  selectedMembership: MembershipDto | null;
  memberships: MembershipDto[];
  setSelectedMembershipId: (id: string) => void;
}

const SelectedMembershipContext = createContext<SelectedMembershipContextType | undefined>(undefined);

interface SelectedMembershipProviderProps {
  currentUser: CurrentUserResponse;
  children: React.ReactNode;
}

export function SelectedMembershipProvider({
  currentUser,
  children,
}: SelectedMembershipProviderProps) {
  const queryClient = useQueryClient();
  const memberships = (currentUser.memberships || []) as MembershipDto[];

  const [selectedId, setSelectedId] = useState<string | null>(() => {
    return memberships.length > 0 && memberships[0].id ? memberships[0].id : null;
  });

  // Ensure selectedId stays valid if memberships change
  useEffect(() => {
    if (memberships.length > 0) {
      if (!selectedId || !memberships.some((m) => m.id === selectedId)) {
        setSelectedId(memberships[0].id || null);
      }
    } else {
      setSelectedId(null);
    }
  }, [memberships, selectedId]);

  const handleSelectMembershipId = (newId: string) => {
    if (newId !== selectedId) {
      // Invalidate business query caches when switching active membership
      queryClient.invalidateQueries({ queryKey: ["customers"] });
      setSelectedId(newId);
    }
  };

  const selectedMembership =
    memberships.find((m) => m.id === selectedId) || (memberships.length > 0 ? memberships[0] : null);

  return (
    <SelectedMembershipContext.Provider
      value={{
        selectedMembership,
        memberships,
        setSelectedMembershipId: handleSelectMembershipId,
      }}
    >
      {children}
    </SelectedMembershipContext.Provider>
  );
}

export function useSelectedMembership(): SelectedMembershipContextType {
  const context = useContext(SelectedMembershipContext);
  if (!context) {
    throw new Error("useSelectedMembership must be used within a SelectedMembershipProvider");
  }
  return context;
}
