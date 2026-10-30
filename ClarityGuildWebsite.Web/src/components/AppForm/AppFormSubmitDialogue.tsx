import { Description, Dialog, DialogPanel, DialogTitle } from '@headlessui/react'
import { dialogClass, textBodyFullClass, appFormButtonClass, fieldTitleClass, fieldHintClass } from '../styles/applicationClasses';
import { DiscordContactNotice } from '../DiscordContactNotice';

type Status = "success" | "error" | "serverError" | "signInTimeOut" | "errorTooManyRequests";
type Props = { status: Status | null; submissionDate: string | null; onClose: () => void };
type DialogContent = { title: string; message: string; disclaimer?: string };

const content: Record<Status, DialogContent> = {
  success: {
    title: "Application submitted!",
    message: "We will review your application as soon as possible."
  },
  error: {
    title: "Something went wrong",
    message: "Please contact cckraken17 on Discord for assistance.",
    disclaimer: "We apologize for the inconvenience and appreciate your understanding."
  },
  serverError: {
    title: "Problem with the server",
    message: "We could not connect to the server. Please try again later.",
    disclaimer: "If the problem persists, contact cckraken17 on Discord for assistance."
  },
  errorTooManyRequests: {
    title: "Too many requests",
    message: "You have sent too many requests in a short period of time. Please wait and try again later.",
    disclaimer: "If the problem persists, contact cckraken17 on Discord for assistance."
  },
  signInTimeOut: {
    title: "Discord authentication timed out",
    message: "Please sign in again.",
    disclaimer: "If the problem persists, contact cckraken17 on Discord for assistance."
  }
};

export function AppFormSubmitDialogue({ status, submissionDate, onClose }: Props) {
const current = status ? content[status] : null;

    return (
      <Dialog open={status !== null} onClose={onClose} className="relative z-50">
       <div className="fixed inset-0 bg-black/70" aria-hidden="true"/>
        <div className="fixed inset-0 flex items-center justify-center p-4">
          <DialogPanel className={`w-full max-w-md max-h-[calc(100dvh-2rem)] overflow-y-auto ${dialogClass}`}>
            <DialogTitle className={fieldTitleClass}>{current?.title}</DialogTitle>
            <Description className={textBodyFullClass}>{current?.message}</Description>
            {current?.disclaimer && <Description className={textBodyFullClass}>{current?.disclaimer}</Description>}
            {status === "success" && submissionDate && (
              <Description className={fieldHintClass}>
                {`Submitted: ${new Date(submissionDate).toLocaleString(undefined, { dateStyle: 'short', timeStyle: 'short' })}`}
              </Description>
            )}
            {status === "success" && (
              <DiscordContactNotice inviteUrl={import.meta.env.VITE_DISCORD_INVITE_URL} />
            )}
            <div className="flex justify-end pt-2">
              <button onClick={onClose} className={`w-full sm:w-auto ${appFormButtonClass}`} type="button">
                Finish
              </button>
            </div>
          </DialogPanel>
        </div>
      </Dialog>
    )

}