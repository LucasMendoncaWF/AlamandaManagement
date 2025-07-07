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
  Boolean = 4,
  Options = 5,
  OptionsArray = 6,
  TextArea = 7,
  Image = 8,
  ImageArray = 9,
}

export type FieldType = File | string | number | null | string[] | File[];