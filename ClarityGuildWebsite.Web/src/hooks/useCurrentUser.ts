import { useState, useEffect } from "react";
import { getMe, logOut, type CurrentUser } from "../api/auth";

type AuthState =
  | { status: 'loading' }
  | { status: 'signedOut' }
  | { status: 'signedIn'; user: CurrentUser }
  | { status: 'error' };

export function useCurrentUser() {
  const [state, setState] = useState<AuthState>({ status: 'loading' });
  const [signOutError, setSignOutError] = useState<string | null>(null);
  
  useEffect(() => {
    getMe()
      .then(user => setState(user ? { status: 'signedIn', user } : { status: 'signedOut' }))
      .catch(() => setState({ status: 'error' }));
  }, []);

async function signOut() {
  setSignOutError(null);
  try {
  await logOut();
  setState({ status: 'signedOut'});
} catch {
  setSignOutError("Failed to sign out. Please try again.")
}
}

function markSignedOut() {
  setSignOutError(null);
  setState({ status: "signedOut" });
}

  return { state, signOut, signOutError, markSignedOut };
}
