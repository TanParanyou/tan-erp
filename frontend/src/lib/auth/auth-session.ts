import {
  signInWithEmailAndPassword,
  signOut as firebaseSignOut,
  onAuthStateChanged,
  type User,
} from "firebase/auth";
import type { QueryClient } from "@tanstack/react-query";
import { auth } from "./firebase-client";
import { queryClient as defaultQueryClient } from "@/lib/query/query-client";

export async function signInWithEmail(email: string, password: string):Promise<User> {
  const userCredential = await signInWithEmailAndPassword(auth, email.trim(), password);
  return userCredential.user;
}

export async function signOutSession(client: QueryClient = defaultQueryClient): Promise<void> {
  // 1. Cancel ongoing queries
  await client.cancelQueries();
  // 2. Clear cache completely so previous user's data never leaks to next user
  client.clear();
  // 3. Sign out of Firebase
  await firebaseSignOut(auth);
}

export async function getAuthToken(forceRefresh = false): Promise<string | null> {
  const currentUser = auth.currentUser;
  if (!currentUser) {
    return null;
  }
  return currentUser.getIdToken(forceRefresh);
}

export function subscribeToAuthChanges(
  callback: (user: User | null) => void,
  client: QueryClient = defaultQueryClient
): () => void {
  let previousUid: string | null = null;

  return onAuthStateChanged(auth, (user) => {
    if (previousUid && (!user || user.uid !== previousUid)) {
      // User changed or logged out: clear queries immediately
      client.clear();
    }
    previousUid = user?.uid ?? null;
    callback(user);
  });
}
