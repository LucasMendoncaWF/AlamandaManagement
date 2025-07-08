<style lang="scss" scoped>
@use '@/assets/variables.scss' as *;
.form-loader {
  z-index: 999;
  position: fixed;
  display: flex;
  justify-self: center;
  align-items: center;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  background-color: rgba($secondary, 0.3);
}

.form {
  .inputs-area {
    display: flex;
    align-items: end;
    justify-content: start;
    flex-wrap: wrap;
    width: 100%;
    column-gap: 15px;
  }

  .translation {
    width: 100%;
    margin-bottom: 10px;

    .language-button {
      all: unset;
      width: 100%;
    }

    .language-title {
      margin: 0;
      border-bottom: 2px solid $primary;
      color: $primary;
      display: flex;
      align-items: center;
      cursor: pointer;

       &:hover {
        opacity: 0.7;
      }

      .triangle {
        width: 6px;
        height: 6px;
        border-right: 2px solid $primary;
        border-bottom: 2px solid $primary;
        margin-top: 2px;
        margin-left: 5px;
        margin-bottom: 5px;
        transform: rotate(45deg);

        &--up {
          margin-bottom: 1px;
          transform: rotate(-135deg);
        }
      }
    }

    .language-area {
      margin-bottom: 15px;
      transition: 0.2s;
    }

    .hidden {
      max-height: 0;
      overflow: auto;
    }

    .collapse-animation {
      max-height: 1000px;
    }
  }

  .form-buttons {
    display: flex;
    padding: 10px 0;
    justify-content: end;
    gap: 8px;
    margin-top: 10px;
    position: sticky;
    background-color: $white;
    bottom: -20px;
    left: 0;
  }

  @keyframes FormAppear {
    0% {
      box-shadow: 0px 0px $black;
    }

    100% {
      box-shadow: -7px 11px $black;
    }
  }
}
</style>

<template>
  <ModalBackground>
    <div class="form-loader" v-if="isFieldsPending">
      <Loader />
    </div>
    <div>
      <ErrorMessage :onClose="onCancel" v-if="isFieldsError" message="An error occurred loading the form" />
    </div>
    <ModalBody>
      <form v-if="!isFieldsError && !isFieldsPending" @submit.prevent="onSubmit" class="form">
        <div class="flex inputs-area">
          <template v-for="field in fields" :key="field.fieldName">
            <DynamicFormField
              :id="field.fieldName"
              :field="field"
              :form="form"
              v-model="form[field.fieldName]"
              :imageField="imageField"
              :isLoading="isLoading"
              :onRemoveImage="onRemoveImage"
              :variant="!(fields && (fields.length > 2)) ? 'double' : ''"
            />
            <div class="form-input-box--double translation" v-if="field.dataType === FieldDataTypeEnum.Translations">
              <div
                v-for="group in field.translationsGroups" 
                :key="group.languageId"
              >
                <button class="language-button" type="button" @click="() => onToggleTranslationFields(group.languageName.toUpperCase())">
                  <h4 class="language-title">
                    {{ group.languageName.toUpperCase() }}
                    <div :class="`triangle ${showTranslations === group.languageName.toUpperCase() ? 'triangle--up' : ''}`"></div>
                  </h4>
                </button>
                <div :class="`inputs-area language-area ${showTranslations === group.languageName.toUpperCase() ? 'collapse-animation' : 'hidden'}`">
                  <template v-for="tField in [...group.fields]?.sort((a, b) => a.dataType - b.dataType)" :key="tField.fieldName">
                    <DynamicFormField
                      :id="`translations_${group.languageId}_${tField.fieldName}`"
                      :field="tField"
                      :form="form"
                      :imageField="imageField"
                      v-model="form[`translations_${group.languageId}_${tField.fieldName}`]"
                      :isLoading="isLoading"
                      :onRemoveImage="onRemoveImage"
                    />
                  </template>
                </div>
              </div>
            </div>
          </template>
        </div>
        <div class="form-buttons">
          <FormButton 
            variant="danger" 
            v-if="data?.id" 
            :disabled="isLoading" 
            @click="() => onDelete(data?.id)"
          >
            Delete
          </FormButton>
          <FormButton variant="inverted" :disabled="isLoading" @click="onCancel">Cancel</FormButton>
          <FormSubmit :disabled="isLoading" :value="data?.id ? 'Save' : 'Create'" />
        </div>

        <div class="form-loader" v-if="isLoading" >
          <Loader />
        </div>
        <ErrorMessage 
          v-if="errorMessage" 
          variant="white" 
          :onClose="onCloseErrorMessage" 
          :message="errorMessage"
        />
      </form>
    </ModalBody>
  </ModalBackground>
</template>

<script lang="ts" setup generic="TRes, TForm">
import { reactive, ref, watch } from 'vue'
import FormSubmit from '@/components/formMaterial/formSubmit.vue'
import ErrorMessage from '@/components/stateHandling/errorMessage.vue'
import Loader from '@/components/stateHandling/loader.vue'
import FormButton from '@/components/formMaterial/formButton.vue'
import { fileToBase64 } from '@/utis/converter'
import { ApiResponseData, getErrorMessage, ResponseKeyType } from '@/api/defaultApi'
import { FormFieldModel, FieldDataTypeEnum, FieldType } from '@/models/formFieldModel'
import ModalBackground from '@/components/modals/modalBackground.vue'
import ModalBody from '@/components/modals/modalBody.vue'
import DynamicFormField from './dynamicFormField.vue'

type ReactiveForm = Record<string, FieldType>;

interface Props {
  data?: ApiResponseData;
  onComplete: () => void;
  onCancel: () => void;
  onDelete: (id?: number) => void;
  addItemFunction: (data: TForm) => Promise<TRes>;
  updateItemFunction: (data: TForm) => Promise<TRes>;
  fields?: FormFieldModel[];
  maxImageSize?: number;
  isFieldsError?: boolean;
  isFieldsPending?: boolean;
}

const props = defineProps<Props>();

const form = reactive<ReactiveForm>({});
const imageField = reactive<Record<string, string | null>>({});
const isLoading = ref(false);
const showTranslations = ref('PT');
const errorMessage = ref<string | null>(null);
const maxSize = props.maxImageSize || 500;

const onToggleTranslationFields = (value: string) => {
  if(value === showTranslations.value) {
    showTranslations.value = '';
    return;
  }
  showTranslations.value = value;
}

const onCloseErrorMessage = () => {
  errorMessage.value = null;
};

const onSubmit = async () => {
  errorMessage.value = null;
  if (!props.fields) { return; }

  for (const f of props.fields) {
    const val = form[f.fieldName];
    if(!isImagesSizeCorrect(f, val)) {
      return;
    }
  }

  isLoading.value = true;

  try {
    const sendData: Record<string, unknown> = {};

    for (const f of props.fields) {
      if (f.dataType === FieldDataTypeEnum.Image) {
        sendData[f.fieldName] = form[f.fieldName]
          ? await fileToBase64(form[f.fieldName] as File)
          : imageField[f.fieldName] || null;
      } else if (f.dataType === FieldDataTypeEnum.ImageArray) {
        if (Array.isArray(form[f.fieldName])) {
          const arrBase64: string[] = [];
          for (const file of form[f.fieldName] as File[]) {
            if (file) { arrBase64.push(await fileToBase64(file)); }
          }
          sendData[f.fieldName] = arrBase64;
        } else {
          sendData[f.fieldName] = [];
        }
      } else if (f.dataType === FieldDataTypeEnum.OptionsArray) {
        sendData[f.fieldName + 'Ids'] = form[f.fieldName] as ResponseKeyType;
      } else if (f.dataType === FieldDataTypeEnum.Translations && Array.isArray(f.translationsGroups)) {
        const translations: any[] = [];

        for (const group of f.translationsGroups) {
          const translationItem: Record<string, FieldType> = {
            languageId: group.languageId
          };

          for (const tf of group.fields) {
            const key = `translations_${group.languageId}_${tf.fieldName}`;
            if(!isImagesSizeCorrect(tf, form[key])) {
              return;
            }
            if(tf.dataType === FieldDataTypeEnum.ImageArray) {
              const arrBase64: string[] = [];
              if(form[key]) {
                for (const file of form[key] as (string | File)[]) {
                  if(typeof file === 'string') {
                    arrBase64.push(file);
                  } else if (file) { 
                    arrBase64.push(await fileToBase64(file));
                  }
                }
              }
              translationItem[tf.fieldName] = arrBase64.length ? arrBase64 : null;
            } else {
              translationItem[tf.fieldName] = form[key] ?? null;
            }
          }

          translations.push(translationItem);
        }

        sendData[f.fieldName] = translations;
      } else if (f.dataType === FieldDataTypeEnum.Date && typeof form[f.fieldName] === 'string') {
        
        sendData[f.fieldName] = convertDate(form[f.fieldName]) as ResponseKeyType;
      } else {
        sendData[f.fieldName] = form[f.fieldName] as ResponseKeyType;
      }
    }
    if (props.data?.id) {
      await props.updateItemFunction({ ...sendData, id: props.data.id } as TForm);
    } else {
      await props.addItemFunction(sendData as TForm);
    }

    props.onComplete();
  } catch (e) {
    errorMessage.value = getErrorMessage(e) || 'Error on sending the data.';
    setTimeout(() => {
      errorMessage.value = '';
    }, 6000);
  } finally {
    isLoading.value = false;
  }
};

const isImagesSizeCorrect = (field: FormFieldModel, value?: FieldType) => {
  if (
    field.dataType === FieldDataTypeEnum.Image ||
      field.dataType === FieldDataTypeEnum.ImageArray
  ) {
    const files = Array.isArray(value) ? value : [value];
    for (const file of files) {
      if (file && file instanceof File && file.size / (1024 * 1024) > maxSize / 1000) {
        errorMessage.value = `The max size for pictures is ${maxSize}KB`;
        return false;
      }
    }
  }
  return true;
}

const convertDate = (value: FieldType): string | null => {
  if (!value || typeof value !== 'string') {
    return null;
  }
  const date = new Date(value);
  date.setTime(date.getTime() + 5 * 60 * 60 * 1000);
  return date.toISOString();
}

const onRemoveImage = (key: string) => {
  imageField[key] = null;
};

watch(
  () => props.data,
  (newData) => {
    if (!newData?.id) {
      for (const key in form) {
        form[key] = null;
      }
      return;
    }
    const typedData = newData as Record<string, unknown>;
    for (const field of props.fields ?? []) {
      const key = field.fieldName;

      if (field.dataType === FieldDataTypeEnum.OptionsArray) {
        const arr = typedData[key];
        if (Array.isArray(arr)) {
          form[key] = arr
            .filter((item) => typeof item === 'object' && item !== null && 'id' in item)
            .map((item) => {
              const obj = item as { id: number | string };
              return obj.id.toString();
            });
        } else {
          form[key] = [];
        }
      } else if (field.dataType === FieldDataTypeEnum.Date) {
        if(typedData[key] && typeof typedData[key] === 'string') {
          const date = new Date(typedData[key]);
          const year = date.getFullYear();
          const month = String(date.getMonth() + 1).padStart(2, '0');
          const day = String(date.getDate()).padStart(2, '0');
          form[key] = `${year}-${month}-${day}`;
        } else {
          form[key] = null;
        }
      } else if (field.dataType === FieldDataTypeEnum.Image) {
        form[key] = null;
        const val = typedData[key];
        if (typeof val === 'string') {
          imageField[key] = val;
        } else {
          imageField[key] = null;
        }
      } else if (field.dataType === FieldDataTypeEnum.Translations && Array.isArray(typedData[key])) {
        const translations = typedData[key] as Array<Record<string, any>>;

        for (const group of field.translationsGroups ?? []) {
          const match = translations.find(t => t.languageId?.toString() === group.languageId?.toString());
          if (!match) { continue; }

          for (const tField of group.fields) {
            const value = match[tField.fieldName];
            form[`translations_${group.languageId}_${tField.fieldName}`] = value ?? null;
          }
        }
      } else {
        form[key] = typedData[key] as FieldType;
      }
    }
  },
  { immediate: true }
);
</script>