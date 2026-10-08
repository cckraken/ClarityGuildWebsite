import { cardClass, fieldTitleClass, textBodyFullClass, appFormButtonClass } from '../components/styles/applicationClasses';

export function PrivacyNotice() {
    return (
        <div className="grid grid-cols-1 gap-4">
        <div className={cardClass}>
            <h1 className={fieldTitleClass}>Privacy Notice</h1>
            <p className={textBodyFullClass}>
                We collect and use your personal information only as necessary to process your application and communicate with you regarding your application status.
            </p>
            <p className={textBodyFullClass}>
                We do not share your personal information with third parties without your consent, except as required by law.
            </p>
            <p className={textBodyFullClass}>
                We hold your personal information for no longer than 6 months, but if you would like this deleted sooner, please contact cckraken17 on Discord.
            </p>           
        </div>
        <button className={appFormButtonClass} type="button" onClick={() => window.history.back()}>
                Back
            </button>
        </div>
    );
}