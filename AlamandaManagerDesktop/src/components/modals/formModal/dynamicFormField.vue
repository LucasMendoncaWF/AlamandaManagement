<style lang="scss" scoped>
  .form-input-box {
    width: calc((100% / 3) - 10px);
    margin-bottom: 10px;

    &--double {
      width: 100%;
    }

    &--image {
      margin-bottom: 5px;
      width: 100%;

      .image-input-box {
        width: 50%;
      }
    }

    &--textarea {
      width: 100%;
    }
  }
</style>

<template>
  <div v-if="isSimpleField(field)" :class="`form-input-box form-input-box--${variant}`">
    <FormInput
      :id="field.fieldName"
      :label="field.fieldName"
      :maxlength="field.fieldMaxSize ? Number(field.fieldMaxSize) : undefined"
      :type="getInputType(field)"
      :required="field.isRequired"
      :disabled="isLoading"
      variant="inverted"
      v-model="internalValue"
    />
  </div>

  <div class="form-input-box" v-else-if="field.dataType === FieldDataTypeEnum.Boolean">
    <FormCheckBox
      :id="field.fieldName"
      :label="field.fieldName"
      :required="field.isRequired"
      :disabled="isLoading"
      variant="inverted"
      v-model="internalValue"
    />
  </div>

  <div class="form-input-box form-input-box--double" v-else-if="field.dataType === FieldDataTypeEnum.OptionsArray">
    <FormMultipleSelect
      :id="field.fieldName"
      :label="field.fieldName"
      :options="field.optionsArray"
      :required="field.isRequired"
      :disabled="isLoading"
      variant="inverted"
      v-model="internalValue"
    />
  </div>

  <div class="form-input-box" v-else-if="field.dataType === FieldDataTypeEnum.Options">
    <FormSelect
      :id="field.fieldName"
      :label="field.fieldName"
      :options="field.optionsArray"
      :required="field.isRequired"
      :disabled="isLoading"
      variant="inverted"
      v-model="internalValue"
    />
  </div>

  <div class="form-input-box--image" v-else-if="field.dataType === FieldDataTypeEnum.Image">
    <div class="image-input-box">
      <FormInputImage
        :id="field.fieldName"
        :label="field.fieldName"
        :previousImage="imageField[field.fieldName]"
        :onRemoveImage="onRemoveImage"
        :disabled="isLoading"
        variant="inverted"
        v-model="internalValue"
      />
    </div>
  </div>

  <div class="form-input-box--textarea" v-else-if="field.dataType === FieldDataTypeEnum.TextArea">
    <FormTextArea
      :id="field.fieldName"
      :label="field.fieldName"
      :disabled="isLoading"
      v-model="internalValue"
      variant="inverted"
    />
  </div>
</template>

<script lang="ts" setup>
import FormCheckBox from '@/components/formMaterial/formCheckBox.vue';
import FormInput from '@/components/formMaterial/formInput.vue';
import FormInputImage from '@/components/formMaterial/formInputImage.vue';
import FormMultipleSelect from '@/components/formMaterial/formMultipleSelect.vue';
import FormSelect from '@/components/formMaterial/formSelect.vue';
import FormTextArea from '@/components/formMaterial/formTextArea.vue';
import { FieldDataTypeEnum, FieldType, FormFieldModel } from '@/models/formFieldModel'

const props = defineProps<{
  field: FormFieldModel;
  form: Record<string, any>;
  imageField: Record<string, string | null>;
  isLoading: boolean;
  onRemoveImage: (key: string) => void;
  boxClass?: string;
  modelValue?: FieldType;
  variant?: 'double' | '';
}>();

const isSimpleField = (field: FormFieldModel) =>
  [FieldDataTypeEnum.Number, FieldDataTypeEnum.String, FieldDataTypeEnum.Date].includes(field.dataType);

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

import { watch, ref } from 'vue';

const internalValue = ref(props.modelValue);

watch(() => props.modelValue, (val) => {
  internalValue.value = val;
});

watch(() => internalValue.value, (val) => {
  emit('update:modelValue', val as FieldType);
});

const emit = defineEmits<{
  (e: 'update:modelValue', value: FieldType): void;
}>();

</script>
