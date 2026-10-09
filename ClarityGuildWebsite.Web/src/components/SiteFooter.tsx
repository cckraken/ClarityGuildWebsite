import { footerContainerClass, footerItemClass, footerLinkClass } from "./styles/applicationClasses";
import { Link } from "react-router";

export function SiteFooter() {
    return (

<footer className ={footerContainerClass}> 
    <p className={footerItemClass}>&copy; 2026 cckraken</p>
    <p className={footerItemClass}>All rights reserved.</p>
    <Link className={footerLinkClass} to="/privacy">Privacy Notice</Link>    
</footer>
    );
}