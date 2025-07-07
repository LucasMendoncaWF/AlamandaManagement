export interface FormFieldModel {
  fieldName: string;
  dataType: FieldDataTypeEnum;
  fieldMaxSize: string;
  optionsArray: FormFieldOptionModel[];
  isRequired: boolean;
  translationsGroups: {
    languageId: number;
    languageName: string;
    fields: FormFieldModel[]
  }[];
}

export interface FormFieldOptionModel {
  id: string;
  name: string;
}

export enum FieldDataTypeEnum {
  Translations = 0,
  String = 1,
  Number = 2,
  Date = 3,
  Options = 4,
  OptionsArray = 5,
  Image = 6,
  ImageArray = 7,
}