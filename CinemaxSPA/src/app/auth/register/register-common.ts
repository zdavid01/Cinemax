type FieldType = "username" | "firstName" | "lastName" | "password" | "email";

const ErrorsForField = new Map(Object.entries({
    "username": ["UserName", "DuplicateUserName"],
    "firstName": ["FirstName"],
    "lastName": ["LastName"],
    "password": ["Password", "PasswordRequiresLower", "PasswordRequiresUpper", "PasswordTooShort", "PasswordRequiresDigit", "PasswordRequiresNonAlphanumeric"],
    "email": ["Email", "DuplicateEmail"]
}
));

export const errorForFormField = (fieldName: FieldType, errors: Map<string, Array<string>>): string | undefined => {
    const keysForField = ErrorsForField.get(fieldName);
    if (keysForField !== undefined) {
        const errorForField = keysForField.flatMap(key => errors.get(key));
        return errorForField.find(x => x !== undefined && x !== "");
    }
    return "";
}

export const formFieldState = (formFieldName: FieldType, errors: Map<string, Array<string>>) => {
    const message = errorForFormField(formFieldName, errors);
    return {
        invalid: Boolean(message),
        message
    }
}