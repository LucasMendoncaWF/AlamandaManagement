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
  Image = 3,
  ImageArray = 4,
  OptionsArray = 5,
  Options = 6
}