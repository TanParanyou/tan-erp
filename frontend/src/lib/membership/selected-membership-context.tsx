"use client";

import React, { createContext, useContext, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import type { MembershipDto } from "@/lib/permissions/can";

interface SelectedMembershipContextType {
  currentUser: CurrentUserResponse;
  selectedMembership: MembershipDto | null;
  memberships: MembershipDto[];
  setSelectedMembershipId: (id: string) => Promise<void>;
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

  const [requestedId, setRequestedId] = useState<string | null>(null);
  const selectedMembership =
    memberships.find((membership) => membership.id === requestedId) ?? memberships[0] ?? null;

  const handleSelectMembershipId = async (newId: string): Promise<void> => {
    if (!memberships.some((membership) => membership.id === newId)) {
      return;
    }
    if (newId === selectedMembership?.id) {
      return;
    }
    await queryClient.cancelQueries({ queryKey: ["business"] });
    queryClient.removeQueries({ queryKey: ["business"] });
    setRequestedId(newId);
  };

  return (
    <SelectedMembershipContext.Provider
      value={{
        currentUser,
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

export function useOptionalSelectedMembership(): SelectedMembershipContextType | null {
  const context = useContext(SelectedMembershipContext);
  return context ?? null;
}
