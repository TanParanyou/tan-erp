//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_invoice_action_request.g.dart';

/// SupplierInvoiceActionRequest
///
/// Properties:
/// * [note] 
@BuiltValue()
abstract class SupplierInvoiceActionRequest implements Built<SupplierInvoiceActionRequest, SupplierInvoiceActionRequestBuilder> {
  @BuiltValueField(wireName: r'note')
  String? get note;

  SupplierInvoiceActionRequest._();

  factory SupplierInvoiceActionRequest([void updates(SupplierInvoiceActionRequestBuilder b)]) = _$SupplierInvoiceActionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierInvoiceActionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierInvoiceActionRequest> get serializer => _$SupplierInvoiceActionRequestSerializer();
}

class _$SupplierInvoiceActionRequestSerializer implements PrimitiveSerializer<SupplierInvoiceActionRequest> {
  @override
  final Iterable<Type> types = const [SupplierInvoiceActionRequest, _$SupplierInvoiceActionRequest];

  @override
  final String wireName = r'SupplierInvoiceActionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierInvoiceActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierInvoiceActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierInvoiceActionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SupplierInvoiceActionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierInvoiceActionRequestBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


