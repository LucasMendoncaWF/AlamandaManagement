import { FieldType } from '@/models/formFieldModel';

export function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();

    reader.onload = () => {
      resolve(reader.result as string);
    };

    reader.onerror = error => reject(error);

    reader.readAsDataURL(file);
  });
}

export function convertFieldNameToLabel(value: string) {
  return value.toLowerCase().replace('_', ' ').replace('id', '');
}

export function formatDateWithoutTime(dateString: FieldType, daysToAdd?: number) {
  if(typeof dateString !== 'string' ) {
    return '';
  }
  const date = new Date(dateString);
  return new Date(date.getFullYear(), date.getMonth(), date.getDate() + (daysToAdd || 0))
    .toLocaleDateString('pt-BR');
}