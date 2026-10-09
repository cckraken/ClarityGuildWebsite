import { cardClass, fieldTitleClass, textBodyFullClass, appFormButtonClass } from '../components/styles/applicationClasses';
import { useNavigate } from 'react-router';

export function PrivacyNotice() {
    const navigate = useNavigate();
    return (
        <div className="grid grid-cols-1 gap-4">
        <div className={cardClass}>
            <h1 className={fieldTitleClass}>Privacy Notice</h1>
            <p className={textBodyFullClass}>
                We collect and use whatever information you provide in the application only as necessary to process your application and communicate with you regarding your application status.
                This data is stored securely on Azure and only officers of the guild will ever see it.
            </p>
            <p className={textBodyFullClass}>
                We do not share your personal information with third parties without your consent, except as required by law.
            </p>
            <p className={textBodyFullClass}>
                Your information is automatically deleted after 3 months, but if you would like this deleted sooner, please contact cckraken17 on Discord.
            </p>           
        </div>
        <button className={appFormButtonClass} type="button" onClick={() => navigate(-1)}>
                Back
            </button>
        </div>
    );
}