<style lang="scss" scoped>
@use '@/assets/variables.scss' as *;
.form-loader {
  z-index: 999;
  position: absolute;
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
    justify-content: space-between;
    flex-wrap: wrap;
    width: 100%;
    gap: 15px;
  }

  .form-input-box {
    width: calc((100% / 3) - 10px);
    margin-bottom: 10px;

    &--double {
      flex-grow: 1;
    }

    &--image {
      margin-bottom: 5px;
    }
  }

  .form-buttons {
    display: flex;
    justify-content: end;
    gap: 8px;
    margin-top: 10px;
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
            <div :class="`form-input-box ${fields && (fields.length > 2) ? '' : 'form-input-box--double'}`" v-if="isSimpleField(field)">
              <FormInput
                :id="field.fieldName"
                :label="field.fieldName"
                v-model="form[field.fieldName] as string"
                :maxlength="field.fieldMaxSize ? Number(field.fieldMaxSize) : undefined"
                :type="getInputType(field)"
                variant="inverted"
                :accept="(field.dataType === FieldDataTypeEnum.Image || field.dataType === FieldDataTypeEnum.ImageArray) ? 'image/*' : undefined"
                :multiple="field.dataType === FieldDataTypeEnum.ImageArray || field.dataType === FieldDataTypeEnum.OptionsArray"
                :required="field.isRequired"
                :disabled="isLoading"
              />
            </div>
            <div class="form-input-box form-input-box--double" v-if="field.dataType === FieldDataTypeEnum.OptionsArray">
              <FormMultipleSelect
                :id="field.fieldName"
                :label="field.fieldName"
                v-model="form[field.fieldName] as string[]"
                @update:modelValue="form[field.fieldName] = $event"
                :options="field.optionsArray"
                variant="inverted"
                :required="field.isRequired"
                :disabled="isLoading"
              />
            </div>

            <div class="form-input-box" v-if="field.dataType === FieldDataTypeEnum.Options">
              <FormSelect
                :id="field.fieldName"
                :label="field.fieldName"
                v-model="form[field.fieldName] as string"
                @update:modelValue="form[field.fieldName] = $event"
                :options="field.optionsArray"
                variant="inverted"
                :required="field.isRequired"
                :disabled="isLoading"
              />
            </div>

            <div class="form-input-box--image" v-if="field.dataType === FieldDataTypeEnum.Image">
              <FormInputImage
                :id="field.fieldName"
                :label="field.fieldName"
                v-model="form[field.fieldName] as File"
                @update:modelValue="form[field.fieldName] = $event"
                :previousImage="imageField[field.fieldName]"
                :onRemoveImage="onRemoveImage"
                variant="inverted"
                :disabled="isLoading"
              />
            </div>
          </template>
        </div>
        <div class="form-buttons">
          <FormButton variant="danger" v-if="data?.id" :disabled="isLoading" @click="() => onDelete(data?.id)">Delete</FormButton>
          <FormButton variant="inverted" :disabled="isLoading" @click="onCancel">Cancel</FormButton>
          <FormSubmit :disabled="isLoading" :value="data?.id ? 'Save' : 'Create'" />
        </div>

        <div class="form-loader" v-if="isLoading" >
          <Loader />
        </div>
        <ErrorMessage variant="white" :onClose="onCloseErrorMessage" v-if="errorMessage" :message="errorMessage" />
      </form>
    </ModalBody>
  </ModalBackground>
</template>

<script lang="ts" setup generic="TRes, TForm">
  import { reactive, ref, watch } from 'vue'
  import FormInput from '@/components/forms/formInput.vue'
  import FormSubmit from '@/components/forms/formSubmit.vue'
  import FormInputImage from '@/components/forms/formInputImage.vue'
  import ErrorMessage from '@/components/errorMessage.vue'
  import Loader from '@/components/loader.vue'
  import FormButton from '@/components/forms/formButton.vue'
  import { fileToBase64 } from '@/utis/converter'
  import { ApiResponseData, getErrorMessage, HasId, ResponseKeyType } from '@/api/defaultApi'
  import { FormFieldModel, FieldDataTypeEnum } from '@/models/formFieldModel'
  import FormMultipleSelect from '../forms/formMultipleSelect.vue'
  import FormSelect from '../forms/formSelect.vue'
  import ModalBackground from '../modals/modalBackground.vue'
  import ModalBody from '../modals/modalBody.vue'

  type FieldType = File | string | number | null | string[] | File[];

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
  const errorMessage = ref<string | null>(null);

  const onCloseErrorMessage = () => {
    errorMessage.value = null;
  };

  const isSimpleField = (field: FormFieldModel) => {
    const dataType = field.dataType;
    return [FieldDataTypeEnum.Number, FieldDataTypeEnum.String, FieldDataTypeEnum.Date].includes(dataType);
  };

  const getInputType = (field: FormFieldModel) => {
    switch (field.dataType) {
      case FieldDataTypeEnum.Number:
        return 'number';
      case FieldDataTypeEnum.Date:
        return 'date';
      default:
        return 'text';
    }
  };

  const onSubmit = async () => {
    errorMessage.value = null;
    if (!props.fields) return;

    const maxSize = props.maxImageSize || 400;

    for (const f of props.fields) {
      const val = form[f.fieldName];
      if (
        f.dataType === FieldDataTypeEnum.Image ||
        f.dataType === FieldDataTypeEnum.ImageArray
      ) {
        const files = Array.isArray(val) ? val : [val];
        for (const file of files) {
          if (file && file instanceof File && file.size / (1024 * 1024) > maxSize / 100) {
            errorMessage.value = `The max size for pictures is ${maxSize}KB`;
            return;
          }
        }
      }
    }

    isLoading.value = true;

    try {
      const sendData = {} as Record<string, unknown>;

      for (const f of props.fields) {
        if (f.dataType === FieldDataTypeEnum.Image) {
          sendData[f.fieldName] = form[f.fieldName]
            ? await fileToBase64(form[f.fieldName] as File)
            : imageField[f.fieldName] || null;
        } else if (f.dataType === FieldDataTypeEnum.ImageArray) {
          if (Array.isArray(form[f.fieldName])) {
            const arrBase64: string[] = [];
            for (const file of form[f.fieldName] as File[]) {
              if (file) arrBase64.push(await fileToBase64(file));
            }
            sendData[f.fieldName] = arrBase64;
          } else {
            sendData[f.fieldName] = [];
          }
        } else if (f.dataType === FieldDataTypeEnum.OptionsArray) {
          sendData[f.fieldName + 'Ids'] = form[f.fieldName] as ResponseKeyType;
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
        } else if (field.dataType === FieldDataTypeEnum.Options) {
          const obj = typedData[key.replace('id', '')];
          if (typeof obj === 'object' && obj !== null && 'id' in obj) {
            const objTyped = obj as { id: number | string };
            form[key] = objTyped.id.toString();
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
        } else {
          form[key] = typedData[key] as FieldType;
        }
      }
    },
    { immediate: true }
  );
</script>