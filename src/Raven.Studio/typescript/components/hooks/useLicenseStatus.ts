﻿import { useEffect, useState } from "react";
import licenseModel from "models/auth/licenseModel";

export function useLicenseStatus() {
    const [status, setStatus] = useState<Raven.Server.Commercial.LicenseStatus>(licenseModel.licenseStatus());

    useEffect(() => {
        const subscription = licenseModel.licenseStatus.subscribe(setStatus);

        return () => subscription.dispose();
    }, []);

    return {
        status,
    };
}
