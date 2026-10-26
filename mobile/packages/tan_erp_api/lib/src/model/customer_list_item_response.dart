//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/customer_contact_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_list_item_response.g.dart';

/// CustomerListItemResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [customerType] 
/// * [displayNameTh] 
/// * [displayNameEn] 
/// * [preferredLocale] 
/// * [status] 
/// * [primaryContact] 
/// * [leadSource] 
@BuiltValue()
abstract class CustomerListItemResponse implements Built<CustomerListItemResponse, CustomerListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'customerType')
  String? get customerType;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'displayNameEn')
  String? get displayNameEn;

  @BuiltValueField(wireName: r'preferredLocale')
  String? get preferredLocale;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'primaryContact')
  CustomerContactResponse? get primaryContact;

  @BuiltValueField(wireName: r'leadSource')
  String? get leadSource;

  CustomerListItemResponse._();

  factory CustomerListItemResponse([void updates(CustomerListItemResponseBuilder b)]) = _$CustomerListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerListItemResponse> get serializer => _$CustomerListItemResponseSerializer();
}

class _$CustomerListItemResponseSerializer implements PrimitiveSerializer<CustomerListItemResponse> {
  @override
  final Iterable<Type> types = const [CustomerListItemResponse, _$CustomerListItemResponse];

  @override
  final String wireName = r'CustomerListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.customerType != null) {
      yield r'customerType';
      yield serializers.serialize(
        object.customerType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameTh != null) {
      yield r'displayNameTh';
      yield serializers.serialize(
        object.displayNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameEn != null) {
      yield r'displayNameEn';
      yield serializers.serialize(
        object.displayNameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.preferredLocale != null) {
      yield r'preferredLocale';
      yield serializers.serialize(
        object.preferredLocale,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.primaryContact != null) {
      yield r'primaryContact';
      yield serializers.serialize(
        object.primaryContact,
        specifiedType: const FullType(CustomerContactResponse),
      );
    }
    if (object.leadSource != null) {
      yield r'leadSource';
      yield serializers.serialize(
        object.leadSource,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CustomerListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerListItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'customerType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerType = valueDes;
          break;
        case r'displayNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameTh = valueDes;
          break;
        case r'displayNameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameEn = valueDes;
          break;
        case r'preferredLocale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.preferredLocale = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'primaryContact':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CustomerContactResponse),
          ) as CustomerContactResponse?;
          if (valueDes == null) continue;
          result.primaryContact.replace(valueDes);
          break;
        case r'leadSource':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.leadSource = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CustomerListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerListItemResponseBuilder();
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


