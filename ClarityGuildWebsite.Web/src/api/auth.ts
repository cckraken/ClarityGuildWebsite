export type CurrentUser = { discordId: string; discordName: string };
const baseUrl = import.meta.env.VITE_API_BASE_URL;

export async function getMe(): Promise<CurrentUser | null> {
    const response = await fetch(`${baseUrl}/api/auth/me`, {
        credentials: 'include',
    });

    if (response.status === 401) return null
    if (!response.ok) throw new Error(`getMe failed: ${response.status}`);

    return (await response.json()) as CurrentUser;

}


export async function logOut() {
  const res = await fetch(`${baseUrl}/api/auth/logout`, {
     method: 'POST',
     credentials: 'include' });
  if (!res.ok) throw new Error(`logOut failed: ${res.status}`);
}
