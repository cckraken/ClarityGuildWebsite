import { useState, useEffect } from "react";
import { getMe, logOut, type CurrentUser } from "../api/auth";

type AuthState =
  | { status: 'loading' }
  | { status: 'signedOut' }
  | { status: 'signedIn'; user: CurrentUser }
  | { status: 'error' };

export function useCurrentUser() {
  const [state, setState] = useState<AuthState>({ status: 'loading' });

  useEffect(() => {
    getMe()
      .then(user => setState(user ? { status: 'signedIn', user } : { status: 'signedOut' }))
      .catch(() => setState({ status: 'error' }));
  }, []);

async function signOut() {
  await logOut();
  setState({ status: 'signedOut' });
}

  return { state, signOut };
}