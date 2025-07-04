export interface FormFieldModel {
  fieldName: string;
  dataType: FieldDataTypeEnum;
  fieldMaxSize: string;
  optionsArray: FormFieldOptionModel[];
  isRequired: boolean;
}

export interface FormFieldOptionModel {
  id: string;
  name: string;
}

export enum FieldDataTypeEnum {
  String = 0,
  Number = 1,
  Date = 2,
  Options = 3,
  Image = 4,
  ImageArray = 5,
  OptionsArray = 6,
}