import { Description, Dialog, DialogPanel, DialogTitle } from '@headlessui/react'
import { dialogClass, textBodyFullClass, appFormButtonClass, fieldTitleClass } from '../styles/applicationClasses';

type Props = { status: "success" | "error" | null; onClose: () => void };
const content = {
  success: {
    title: "Application submitted",
    message: "We will review your application as soon as possible. If you don't hear from us within a few days, please assume it has been declined.",
    disclaimer: "In the near future we will have functionality for tracking the status of your application. In the meantime, please contact cckraken17 on Discord with any questions."
  },
  error: {
    title: "Something went wrong",
    message: "Please contact cckraken17 on Discord for assistance.",
    disclaimer: "We apologize for the inconvenience and appreciate your understanding."
  },
};

export function AppFormSubmitDialogue({ status, onClose }: Props) {
const current = status ? content[status] : null;
    return (
      <Dialog open={status !== null} onClose={onClose} className="relative z-50">
       <div className="fixed inset-0 bg-black/70" aria-hidden="true"/>
        <div className="fixed inset-0 flex items-center justify-center p-4">
          <DialogPanel className={`w-full max-w-md ${dialogClass}`}>
            <DialogTitle className={fieldTitleClass}>{current?.title}</DialogTitle>
            <Description className={textBodyFullClass}>{current?.message}</Description>
            {current?.disclaimer && <Description className={textBodyFullClass}>{current?.disclaimer}</Description>}
            <div className="flex gap-4">
              <button onClick={onClose} className={appFormButtonClass}
               type="button"
               >Back</button>
               <button onClick={onClose} className={appFormButtonClass}
               type="button"
               >Finish</button>
            </div>
          </DialogPanel>
        </div>
      </Dialog>
    )

}