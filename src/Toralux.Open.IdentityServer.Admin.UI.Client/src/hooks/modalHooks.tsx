import { useState, useCallback } from "react";

export type UseModalReturn = {
  isOpen: boolean;
  openModal: () => void;
  closeModal: () => void;
  toggleModal: (isOpen?: boolean) => void;
  setValue: (isOpen: boolean) => void;
};

const useModal = (initiallyOpened: boolean = false): UseModalReturn => {
  const [isOpen, setIsOpen] = useState<boolean>(initiallyOpened ?? false);

  const openModal = useCallback(() => {
    setIsOpen(true);
  }, []);

  const closeModal = useCallback(() => {
    setIsOpen(false);
  }, []);

  const toggleModal = useCallback((isOpen?: boolean) => {
    if (typeof isOpen === "boolean") {
      setIsOpen(isOpen);
      return;
    }

    setIsOpen((prevState) => !prevState);
  }, []);

  const setValue = useCallback((isOpen: boolean) => {
    setIsOpen(isOpen);
  }, []);

  return {
    isOpen,
    openModal,
    closeModal,
    toggleModal,
    setValue,
  };
};

export default useModal;
