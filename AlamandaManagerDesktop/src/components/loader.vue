<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  $border-color: $primary;
  $block-color: $white;
  $transit-color: rgba($primary, 0.8);
  $page-color: $white;
  .loader-container{  
    background-color: rgba($secondary, 0.8);
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
    animation: fadeIn 1s;

    .loader {
      margin: 5% auto 30px;
    }

    .book {
      background: $page-color;
      border: 4px solid $border-color;
      width: 66px;
      height: 45px;
      position: relative;
      perspective: 150px;
    }

    .page {
      display: block;
      width: 30px;
      height: 45px;
      border: 4px solid $border-color;
      border-left: 3px solid $transit-color;
      margin: 0;
      position: absolute;
      right: -4px;
      top: -4px;
      overflow: hidden;
      background: $block-color;
      transform-style: preserve-3d;
      transform-origin: left center;

      &:last-of-type {
        &::before {
          display: none;
        }

        &::after {
          display: none;
        }
      }
      &:before{
        content: '';
        width: 55%;
        height: 40%;
        background: $block-color;
        position: absolute;
        top: 10%;
        left: 20%;
        border-radius: 20%;
        z-index: 9;
      }
      &:after{
        content: '';
        width: 55%;
        height: 40%;
        background: $block-color;
        position: absolute;
        top: 55%;
        left: 20%;
        border-radius: 20%;
        z-index: 9;
      }
    }

    .book .page {
      &:nth-child(1) {
        animation: pageTurn 2s cubic-bezier(0, 0.39, 1, 0.68) 1.8s infinite;
        z-index: 9;
      }
      &:nth-child(2) {
        animation: pageTurn 2s cubic-bezier(0, 0.39, 1, 0.68) 1.8s infinite;
        z-index: 96;
      }
      &:nth-child(3) {
        animation: pageTurn 2s cubic-bezier(0, 0.39, 1, 0.68) 2s infinite;
        z-index: 97;
      }
      &:nth-child(4) {
        animation: pageTurn 2s cubic-bezier(0, 0.39, 1, 0.68) 2.2s infinite;
        z-index: 98;
      }
    }
  }

  @keyframes fadeIn {
    0% {
      opacity: 0;
    }

    100% {
      opacity: 1;
    }
  }

  @keyframes pageTurn {
    0% {
      transform: rotateY(0deg);
    }

    20% {
      background: $transit-color;
      border-color: $transit-color;
    }

    40% {
      background: $page-color;
      transform: rotateY(-180deg);
    }

    100% {
      background: $page-color;
      transform: rotateY(-180deg);
    }
  }
</style>

<template>
  <div class="loader-container">
    <div class="loader book">
      <figure class="page"></figure>
      <figure class="page"></figure>
      <figure class="page"></figure>  
      <figure class="page"></figure> 
    </div>
  </div>
</template>